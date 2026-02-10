using System;
using System.Collections.Generic;
using OctaNotes.Scripts.Core.Model;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Judgment;
using OctaNotes.Scripts.Play.Model.Judgment.Processors;
using OctaNotes.Scripts.Settings;
using R3;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.Play.Model
{
    /// <summary>
    /// 判定管理システム
    /// 状態管理とProcessorの統括を担当
    /// </summary>
    public class JudgmentManager : IInitializable, ITickable, IDisposable
    {
        [Inject] private readonly IInputController _inputController;
        [Inject] private readonly PlaySettingsSO _playSettingsSO;
        [Inject] private readonly IChartRepositoryImmutable _chartRepository;

        // 各レーンの判定状態
        private readonly LaneJudgmentState[] _laneStates = new LaneJudgmentState[8];

        // ノーツタイプ別Processor
        private readonly Dictionary<NoteType, INoteProcessor> _processors = new Dictionary<NoteType, INoteProcessor>();

        private readonly Subject<JudgmentEvent> _judgmentEventSubject = new Subject<JudgmentEvent>();
        public Observable<JudgmentEvent> OnJudgmentEvent => _judgmentEventSubject;

        private TimingWindow _timingWindow;
        private CompositeDisposable _disposables = new CompositeDisposable();

        public JudgmentManager()
        {
            Debug.Log("JudgmentManager initialized");

            // レーン状態を初期化
            for (int i = 0; i < 8; i++)
            {
                _laneStates[i] = new LaneJudgmentState();
            }
        }

        public void Initialize()
        {
            Debug.Log("[JudgmentManager] Initialize called");

            // TimingWindowを設定から作成
            _timingWindow = new TimingWindow(
                _playSettingsSO.perfectRangeMs / 1000f,
                _playSettingsSO.goodRangeMs / 1000f,
                _playSettingsSO.badRangeMs / 1000f
            );

            // Processorを初期化
            InitializeProcessors();

            // 各レーンの入力を購読
            for (int lane = 0; lane < 8; lane++)
            {
                int laneIndex = lane; // クロージャ用
                _inputController.IsButtonPressing[lane]
                    .Subscribe(isPressed =>
                    {
                        float currentTime = Time.time;

                        // ロングノーツ中にボタンが離されたらフラグを立てる
                        if (_laneStates[laneIndex].IsInLongNote && !isPressed)
                        {
                            _laneStates[laneIndex].SetLongNoteReleased(true);
                            Debug.Log($"[JudgmentManager] Lane {laneIndex}: Long note released at {currentTime:F3}");
                        }

                        if (isPressed)
                        {
                            bool judged = ProcessButtonPress(laneIndex, currentTime);

                            // 判定が発生しなかった場合、Noneイベントを発行（フィードバック用）
                            if (!judged)
                            {
                                EmitJudgment(laneIndex, JudgmentResult.None, JudgmentType.Tap, currentTime, currentTime);
                            }
                        }
                    })
                    .AddTo(_disposables);
            }

            Debug.Log($"[JudgmentManager] Chart loaded with {_chartRepository.LaneWiseChartData.Count} lanes");
            for (int i = 0; i < 8; i++)
            {
                Debug.Log($"[JudgmentManager] Lane {i}: {_chartRepository.LaneWiseChartData[i].Count} notes");
            }
        }

        private void InitializeProcessors()
        {
            var tapProcessor = new TapNoteProcessor(_timingWindow);
            var longProcessor = new LongNoteProcessor(_timingWindow);
            var chainProcessor = new ChainNoteProcessor(_timingWindow);

            foreach (var noteType in tapProcessor.SupportedNoteTypes)
            {
                _processors[noteType] = tapProcessor;
            }

            foreach (var noteType in longProcessor.SupportedNoteTypes)
            {
                _processors[noteType] = longProcessor;
            }

            foreach (var noteType in chainProcessor.SupportedNoteTypes)
            {
                _processors[noteType] = chainProcessor;
            }
        }

        public void Tick()
        {
            float currentTime = Time.time;

            for (int laneIndex = 0; laneIndex < 8; laneIndex++)
            {
                var state = _laneStates[laneIndex];

                // 保留判定の発火チェック
                if (state.HasPendingJudgment)
                {
                    var pending = state.PendingJudgment.Value;
                    if (pending.IsReadyToFire(currentTime))
                    {
                        EmitJudgment(laneIndex, pending.Result, pending.Type, pending.EvaluatedTime, pending.EffectTime);
                        state.ClearPendingJudgment();
                    }
                }

                var noteList = _chartRepository.LaneWiseChartData[laneIndex];

                if (state.CurrentNoteIndex >= noteList.Count) continue;

                var noteTiming = noteList[state.CurrentNoteIndex];
                var noteTime = (float)(noteTiming.timing + _playSettingsSO.songStartDelay);

                if (!_processors.TryGetValue(noteTiming.noteType, out var processor)) continue;

                var context = CreateContext(laneIndex, currentTime, noteTime, noteTiming.noteType);
                var output = processor.ProcessTick(context);

                if (output.HasValue)
                {
                    ApplyOutput(laneIndex, output.Value);
                }
            }
        }

        private bool ProcessButtonPress(int laneIndex, float currentTime)
        {
            var noteList = _chartRepository.LaneWiseChartData[laneIndex];
            var state = _laneStates[laneIndex];

            if (state.CurrentNoteIndex >= noteList.Count) return false;

            var noteTiming = noteList[state.CurrentNoteIndex];
            var noteTime = (float)(noteTiming.timing + _playSettingsSO.songStartDelay);

            if (!_processors.TryGetValue(noteTiming.noteType, out var processor)) return false;

            var context = CreateContext(laneIndex, currentTime, noteTime, noteTiming.noteType);
            var output = processor.ProcessPress(context);

            if (output.HasValue)
            {
                ApplyOutput(laneIndex, output.Value);
                return true;
            }

            return false;
        }

        private NoteProcessorContext CreateContext(int laneIndex, float currentTime, float noteTime, NoteType noteType)
        {
            return new NoteProcessorContext(
                _laneStates[laneIndex],
                currentTime,
                noteTime,
                _timingWindow.GetMissThreshold(),
                _inputController.IsButtonPressing[laneIndex].Value,
                noteType,
                laneIndex
            );
        }

        private void ApplyOutput(int laneIndex, JudgmentOutput output)
        {
            var state = _laneStates[laneIndex];

            // 状態更新を適用
            if (output.StateUpdate.HasValue)
            {
                output.StateUpdate.Value.ApplyTo(state);
            }

            // ノーツを進める
            if (output.ShouldAdvanceNote)
            {
                state.AdvanceNote();
            }

            // 遅延発火の場合は保留判定として保存
            if (output.IsDelayed)
            {
                state.SetPendingJudgment(output.ToPendingJudgment(laneIndex));
                Debug.Log($"[JudgmentManager] Lane {laneIndex}: Pending judgment set, will fire at {output.EffectTime:F3}");
            }
            else
            {
                // 即時発火
                EmitJudgment(laneIndex, output.Result, output.Type, output.EvaluatedTime, output.EffectTime);
            }
        }

        private void EmitJudgment(int laneIndex, JudgmentResult result, JudgmentType type, float evaluatedTime, float effectTime)
        {
            var judgmentEvent = new JudgmentEvent(result, evaluatedTime, effectTime, laneIndex, type);
            Debug.Log($"[Judgment] Lane {laneIndex}: {result} ({type}) at {evaluatedTime:F3}, effect at {effectTime:F3}");
            _judgmentEventSubject.OnNext(judgmentEvent);
        }

        public void Dispose()
        {
            _disposables.Dispose();
            _judgmentEventSubject.Dispose();
        }
    }
}
