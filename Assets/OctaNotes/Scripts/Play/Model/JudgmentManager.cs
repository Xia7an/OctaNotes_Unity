using System;
using System.Collections.Generic;
using OctaNotes.Scripts.Core.Model;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Judgment;
using OctaNotes.Scripts.Settings;
using R3;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.Play.Model
{
    /// <summary>
    /// 判定管理システム
    /// 既存のChartRepository/NoteTiming構造と連携
    /// </summary>
    public class JudgmentManager : IInitializable, ITickable, IDisposable
    {
        [Inject] private readonly IInputController _inputController;
        [Inject] private readonly PlaySettingsSO _playSettingsSO;
        [Inject] private readonly IChartRepositoryImmutable _chartRepository;

        // 各レーンの現在判定中ノーツインデックス
        private readonly int[] _currentNoteIndices = new int[8];

        // 各レーンのロングノーツ開始時刻（ロングノーツ中にのみ有効）
        private readonly float[] _longNoteStartTimes = new float[8];

        // 各レーンがロングノーツ中かどうか
        private readonly bool[] _isInLongNote = new bool[8];

        // 各レーンでロングノーツ中にボタンが離されたかどうか
        private readonly bool[] _longNoteReleased = new bool[8];

        // 各レーンのチェインノーツ判定済みかどうか
        private readonly bool[] _chainNoteJudged = new bool[8];

        // 各レーンのチェインノーツでボタンが押された最初の時刻
        private readonly float[] _chainNoteFirstPressTime = new float[8];

        private readonly Subject<JudgmentEvent> _judgmentEventSubject = new Subject<JudgmentEvent>();
        public Observable<JudgmentEvent> OnJudgmentEvent => _judgmentEventSubject;

        private const float TIMING_WINDOW = 0.15f; // 150ms

        private CompositeDisposable _disposables = new CompositeDisposable();

        public JudgmentManager()
        {
            Debug.Log("JudgmentManager initialized");
            for (int i = 0; i < 8; i++)
            {
                _currentNoteIndices[i] = 0;
                _longNoteStartTimes[i] = 0f;
                _isInLongNote[i] = false;
                _longNoteReleased[i] = false;
                _chainNoteJudged[i] = false;
                _chainNoteFirstPressTime[i] = -1f;
            }
        }

        public void Initialize()
        {
            Debug.Log("[JudgmentManager] Initialize called");

            // 各レーンの入力を購読
            for (int lane = 0; lane < 8; lane++)
            {
                int laneIndex = lane; // クロージャ用
                _inputController.IsButtonPressing[lane]
                    .Subscribe(isPressed =>
                    {
                        float currentTime = Time.time;

                        // ロングノーツ中にボタンが離されたらフラグを立てる
                        if (_isInLongNote[laneIndex] && !isPressed)
                        {
                            _longNoteReleased[laneIndex] = true;
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

        public void Tick()
        {
            float currentTime = Time.time;

            for (int laneIndex = 0; laneIndex < 8; laneIndex++)
            {
                var noteList = _chartRepository.LaneWiseChartData[laneIndex];
                var noteIndex = _currentNoteIndices[laneIndex];

                if (noteIndex >= noteList.Count) continue;

                var noteTiming = noteList[noteIndex];
                var noteTime = (float)(noteTiming.timing + _playSettingsSO.songStartDelay);

                switch (noteTiming.noteType)
                {
                    case NoteType.Tap:
                        ProcessTapNoteTick(laneIndex, currentTime, noteTime);
                        break;
                    case NoteType.LongStart:
                        ProcessLongStartNoteTick(laneIndex, currentTime, noteTime);
                        break;
                    case NoteType.LongEnd:
                        ProcessLongEndNoteTick(laneIndex, currentTime, noteTime);
                        break;
                    case NoteType.Chain:
                        ProcessChainNoteTick(laneIndex, currentTime, noteTime);
                        break;
                }
            }
        }

        private bool ProcessButtonPress(int laneIndex, float currentTime)
        {
            var noteList = _chartRepository.LaneWiseChartData[laneIndex];
            var noteIndex = _currentNoteIndices[laneIndex];

            if (noteIndex >= noteList.Count) return false;

            var noteTiming = noteList[noteIndex];
            var noteTime = (float)(noteTiming.timing + _playSettingsSO.songStartDelay);

            switch (noteTiming.noteType)
            {
                case NoteType.Tap:
                    return ProcessTapNotePress(laneIndex, currentTime, noteTime);
                case NoteType.LongStart:
                    return ProcessLongStartNotePress(laneIndex, currentTime, noteTime);
                case NoteType.Chain:
                    return ProcessChainNotePress(laneIndex, currentTime, noteTime);
                // LongEndはボタン押下では判定しない（ホールド状態で判定）
                default:
                    return false;
            }
        }

        #region Tap Note
        private bool ProcessTapNotePress(int laneIndex, float currentTime, float noteTime)
        {
            float timeDiff = currentTime - noteTime;

            // 早すぎる場合は判定しない
            if (timeDiff < -TIMING_WINDOW) return false;

            JudgmentResult result = EvaluateTiming(timeDiff);
            if (result == JudgmentResult.None) return false;

            EmitJudgment(laneIndex, result, JudgmentType.Tap, currentTime, currentTime);
            _currentNoteIndices[laneIndex]++;
            return true;
        }

        private void ProcessTapNoteTick(int laneIndex, float currentTime, float noteTime)
        {
            float timeDiff = currentTime - noteTime;

            // ウィンドウを過ぎたらMiss
            if (timeDiff > TIMING_WINDOW)
            {
                EmitJudgment(laneIndex, JudgmentResult.Miss, JudgmentType.Tap, currentTime, currentTime);
                _currentNoteIndices[laneIndex]++;
            }
        }
        #endregion

        #region Long Start Note
        private bool ProcessLongStartNotePress(int laneIndex, float currentTime, float noteTime)
        {
            float timeDiff = currentTime - noteTime;

            // 早すぎる場合は判定しない
            if (timeDiff < -TIMING_WINDOW) return false;

            JudgmentResult result = EvaluateTiming(timeDiff);
            if (result == JudgmentResult.None) return false;

            // ロングノーツ開始
            _isInLongNote[laneIndex] = true;
            _longNoteStartTimes[laneIndex] = currentTime;
            _longNoteReleased[laneIndex] = false;

            EmitJudgment(laneIndex, result, JudgmentType.LongStart, currentTime, currentTime);
            _currentNoteIndices[laneIndex]++;

            Debug.Log($"[JudgmentManager] Lane {laneIndex}: Long note started at {currentTime:F3}");
            return true;
        }

        private void ProcessLongStartNoteTick(int laneIndex, float currentTime, float noteTime)
        {
            float timeDiff = currentTime - noteTime;

            // ウィンドウを過ぎたらMiss
            if (timeDiff > TIMING_WINDOW)
            {
                EmitJudgment(laneIndex, JudgmentResult.Miss, JudgmentType.LongStart, currentTime, currentTime);
                _currentNoteIndices[laneIndex]++;
            }
        }
        #endregion

        #region Long End Note
        private void ProcessLongEndNoteTick(int laneIndex, float currentTime, float noteTime)
        {
            // ロングノーツ中でない場合（始点をミスした場合など）は終点もMiss
            if (!_isInLongNote[laneIndex])
            {
                float timeDiff = currentTime - noteTime;
                // ウィンドウを過ぎたらMiss判定して次へ
                if (timeDiff > TIMING_WINDOW)
                {
                    Debug.Log($"[JudgmentManager] Lane {laneIndex}: Long end Miss - not in long note");
                    EmitJudgment(laneIndex, JudgmentResult.Miss, JudgmentType.LongEnd, currentTime, currentTime);
                    _currentNoteIndices[laneIndex]++;
                }
                return;
            }

            // 判定タイミング: (正しい時刻) - 150ms
            float judgmentTime = noteTime - TIMING_WINDOW;

            // まだ判定タイミングに達していない
            if (currentTime < judgmentTime)
            {
                // ロングノーツ中にボタンが離されていたらMiss
                if (_longNoteReleased[laneIndex])
                {
                    Debug.Log($"[JudgmentManager] Lane {laneIndex}: Long end Miss - released before judgment time");
                    EmitJudgment(laneIndex, JudgmentResult.Miss, JudgmentType.LongEnd, currentTime, currentTime);
                    _isInLongNote[laneIndex] = false;
                    _currentNoteIndices[laneIndex]++;
                }
                return;
            }

            // 判定タイミングに達した
            // ボタンが離されていなければPerfect（演出は正しい時刻に発生）
            if (!_longNoteReleased[laneIndex])
            {
                Debug.Log($"[JudgmentManager] Lane {laneIndex}: Long end Perfect at {noteTime:F3}");
                EmitJudgment(laneIndex, JudgmentResult.Perfect, JudgmentType.LongEnd, currentTime, noteTime);
                _isInLongNote[laneIndex] = false;
                _currentNoteIndices[laneIndex]++;
            }
            else
            {
                // 判定タイミング前にボタンが離されていたらMiss
                Debug.Log($"[JudgmentManager] Lane {laneIndex}: Long end Miss - released before judgment time");
                EmitJudgment(laneIndex, JudgmentResult.Miss, JudgmentType.LongEnd, currentTime, currentTime);
                _isInLongNote[laneIndex] = false;
                _currentNoteIndices[laneIndex]++;
            }
        }
        #endregion

        #region Chain Note
        private bool ProcessChainNotePress(int laneIndex, float currentTime, float noteTime)
        {
            if (_chainNoteJudged[laneIndex]) return false;

            float timeDiff = currentTime - noteTime;

            // 早すぎる場合はNone（フィードバックのみ）
            if (timeDiff < -TIMING_WINDOW)
            {
                // None判定 - フィードバック表示
                return false;
            }

            // ウィンドウ内でボタンが押された
            if (timeDiff >= -TIMING_WINDOW && timeDiff <= TIMING_WINDOW)
            {
                _chainNoteJudged[laneIndex] = true;
                _chainNoteFirstPressTime[laneIndex] = currentTime;

                // 演出時刻を計算
                float effectTime;
                if (timeDiff <= 0)
                {
                    // 早入り: 演出は正しい時刻に発生
                    effectTime = noteTime;
                }
                else
                {
                    // 遅入り: 演出はボタンが押された時刻に発生
                    effectTime = currentTime;
                }

                Debug.Log($"[JudgmentManager] Lane {laneIndex}: Chain Perfect at {currentTime:F3}, effect at {effectTime:F3}");
                EmitJudgment(laneIndex, JudgmentResult.Perfect, JudgmentType.Chain, currentTime, effectTime);
                _chainNoteJudged[laneIndex] = false;
                _chainNoteFirstPressTime[laneIndex] = -1f;
                _currentNoteIndices[laneIndex]++;
                return true;
            }

            return false;
        }

        private void ProcessChainNoteTick(int laneIndex, float currentTime, float noteTime)
        {
            if (_chainNoteJudged[laneIndex]) return;

            float timeDiff = currentTime - noteTime;

            // ウィンドウ内でボタンが押されているかチェック
            if (timeDiff >= -TIMING_WINDOW && timeDiff <= TIMING_WINDOW)
            {
                if (_inputController.IsButtonPressing[laneIndex].Value)
                {
                    _chainNoteJudged[laneIndex] = true;

                    // 早入り: 演出は正しい時刻に発生（現在時刻が正しい時刻以前）
                    // 遅入り: 演出は現在時刻に発生
                    float effectTime = (timeDiff <= 0) ? noteTime : currentTime;

                    Debug.Log($"[JudgmentManager] Lane {laneIndex}: Chain Perfect (held) at {currentTime:F3}");
                    EmitJudgment(laneIndex, JudgmentResult.Perfect, JudgmentType.Chain, currentTime, effectTime);
                    _chainNoteJudged[laneIndex] = false;
                    _chainNoteFirstPressTime[laneIndex] = -1f;
                    _currentNoteIndices[laneIndex]++;
                }
            }

            // ウィンドウを過ぎたらMiss
            if (timeDiff > TIMING_WINDOW)
            {
                Debug.Log($"[JudgmentManager] Lane {laneIndex}: Chain Miss - not pressed in window");
                EmitJudgment(laneIndex, JudgmentResult.Miss, JudgmentType.Chain, currentTime, currentTime);
                _chainNoteJudged[laneIndex] = false;
                _chainNoteFirstPressTime[laneIndex] = -1f;
                _currentNoteIndices[laneIndex]++;
            }
        }
        #endregion

        private JudgmentResult EvaluateTiming(float timeDiff)
        {
            float absTimeDiff = Mathf.Abs(timeDiff);

            if (absTimeDiff <= _playSettingsSO.perfectRangeMs / 1000f)
            {
                return JudgmentResult.Perfect;
            }
            if (absTimeDiff <= _playSettingsSO.goodRangeMs / 1000f)
            {
                return JudgmentResult.Good;
            }
            if (absTimeDiff <= _playSettingsSO.badRangeMs / 1000f)
            {
                return JudgmentResult.Bad;
            }

            return JudgmentResult.None;
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
