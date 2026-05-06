using System;
using System.Collections.Generic;
using OctaNotes.Scripts.Play.DI.Lane;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Enum;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.Model.Struct;
using OctaNotes.Scripts.Settings;
using R3;
using Zenject;

namespace OctaNotes.Scripts.Play.Model
{
    /// <summary>
    /// 全レーンのノーツ判定結果を収集し、全ノーツのエフェクト発火タイミング（effectInvokeTiming）到達をもって
    /// 楽曲終了と判定し、ISongEndHandler に通知するクラス。
    /// </summary>
    public class ChartEndDetector : IChartEndDetector, IInitializable, IDisposable, ITickable
    {
        private readonly ILaneSubContainerFactory _laneSubContainerFactory;
        private readonly IChartRepositoryImmutable _chartRepository;
        private readonly IInGameTimer _inGameTimer;
        private readonly IScoreCalcurator _scoreCalcurator;
        private readonly IComboCalcurator _comboCalcurator;
        private readonly PlaySettingsSO _settings;
        
        public ReactiveProperty<SongEndState> OnSongEnd { get; } = new ReactiveProperty<SongEndState>();
        
        private readonly CompositeDisposable _disposables = new();

        private readonly List<ILaneOutputPort> _laneOutputPorts = new(8);
        private readonly Queue<JudgeResult> _waitingJudgeResults = new();

        private int _totalNoteCount = -1;
        private int _finalizedNoteCount;
        private bool _hasFired;

        private float musicLength = 0f;

        public ChartEndDetector(
            ILaneSubContainerFactory laneSubContainerFactory,
            IChartRepositoryImmutable chartRepository,
            IInGameTimer inGameTimer,
            IScoreCalcurator scoreCalcurator,
            IComboCalcurator comboCalcurator,
            PlaySettingsSO settings)
        {
            _laneSubContainerFactory = laneSubContainerFactory;
            _chartRepository = chartRepository;
            _inGameTimer = inGameTimer;
            _scoreCalcurator = scoreCalcurator;
            _comboCalcurator = comboCalcurator;
            _settings = settings;
        }

        public void Initialize()
        {
            // 各レーンのSubContainerからILaneOutputPortを取得
            for (var i = 0; i < 8; i++)
            {
                _laneOutputPorts.Add(
                    _laneSubContainerFactory.GetLaneSubContainer(i).Resolve<ILaneOutputPort>()
                );
            }

            // 全レーンの判定結果を購読し、有効な結果のみキューに積む
            foreach (var laneOutput in _laneOutputPorts)
            {
                laneOutput.JudgeResult
                    .Where(IsValidJudge)
                    .Subscribe(result => _waitingJudgeResults.Enqueue(result))
                    .AddTo(_disposables);
            }
            
            // 楽曲の長さを取得する
        }

        /// <summary>
        /// 毎フレーム、キュー先頭の effectInvokeTiming が現在時刻を過ぎていれば
        /// 最終化済みカウントを進める。全ノーツ完了時に SongEndState を通知する。
        ///
        /// UniTask による非同期処理ではタイミング"ちょうど"にイベントが発火される保証がないため、
        /// Tick による毎フレームチェックで実装する（JudgeSoundViewModel と同様の設計）。
        /// </summary>
        public void Tick()
        {
            if (_hasFired) return;

            if (_totalNoteCount < 0)
            {
                var count = _chartRepository.GetNoteCount();
                if (count > 0)
                {
                    _totalNoteCount = count;
                }
                return;
            }

            if (_waitingJudgeResults.Count == 0) return;

            var currentTime = _inGameTimer.Time.Value;

            while (_waitingJudgeResults.Count > 0 &&
                   _waitingJudgeResults.Peek().effectInvokeTiming <= currentTime)
            {
                _waitingJudgeResults.Dequeue();
                _finalizedNoteCount++;
            }

            if (_finalizedNoteCount >= _totalNoteCount)
            {
                _hasFired = true;

                ClearMark _clearMark;
                if (_scoreCalcurator.Score.Value == _settings.maxScore)
                {
                    _clearMark = ClearMark.Ap;
                }
                else if (_comboCalcurator.Combo.Value == _chartRepository.GetNoteCount())
                {
                    _clearMark = ClearMark.Fc;
                }
                else if (_scoreCalcurator.Score.Value > _settings.maxScore * 0.6)
                {
                    _clearMark = ClearMark.Clear;
                }
                else
                {
                    _clearMark = ClearMark.Failed;
                }

                OnSongEnd.OnNext(new SongEndState
                {
                    clearMark = _clearMark
                });
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private static bool IsValidJudge(JudgeResult result)
        {
            return result.judge != Judge.NotJudged && result.judge != Judge.None;
        }
    }
}
