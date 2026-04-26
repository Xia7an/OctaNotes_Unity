using System.Collections.Generic;
using OctaNotes.Scripts.Play.DI.Lane;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Enum;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.Model.Struct;
using OctaNotes.Scripts.Settings;
using R3;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.Play.Model
{
    public class ScoreCalcurator : IScoreCalcurator, IInitializable
    {
        private ILaneSubContainerFactory _laneSubContainerFactory;
        private IChartRepositoryImmutable _chartRepository;
        private PlaySettingsSO _playSettings;

        public ReactiveProperty<int> Score { get; } = new(0);

        private readonly List<ILaneOutputPort> _laneOutputPorts = new(8);
        private int _perNoteScore; // Perfect時の1ノーツあたりのスコア
        private float _internalScore; // 内部で保持している小数点以下まで考慮したスコア

        private readonly Dictionary<Judge, float> ScoreRate = new()
        {
            [Judge.Perfect] = 1.0f,
            [Judge.Good] = 0.7f,
            [Judge.Bad] = 0.4f,
            [Judge.Miss] = 0f
        };

        private int perfectCount = 0;
        private int goodCount = 0;
        private int badCount = 0;
        private int missCount = 0;

        private CompositeDisposable _disposables = new();

        public ScoreCalcurator(ILaneSubContainerFactory laneSubContainerFactory,
            IChartRepositoryImmutable chartRepository, PlaySettingsSO playSettings)
        {
            _laneSubContainerFactory = laneSubContainerFactory;
            _chartRepository = chartRepository;
            _playSettings = playSettings;
        }

        public void Initialize()
        {
            SetPerNoteScore();
            
            // 各レーンのSubContainerからILaneOutputPortを取得
            for (var i = 0; i < 8; i++)
            {
                _laneOutputPorts.Add(_laneSubContainerFactory.GetLaneSubContainer(i).Resolve<ILaneOutputPort>());
            }

            // 各レーンごとにイベント購読
            foreach (var laneOutput in _laneOutputPorts)
            {
                laneOutput.JudgeResult
                    .Subscribe(HandleJudge).AddTo(_disposables);
            }
        }

        private void SetPerNoteScore()
        {
            _perNoteScore = _playSettings.maxScore / _chartRepository.GetNoteCount();
            Debug.Log($"{_perNoteScore} per note score");
        }
        

        private float CalcScore(
            int perfectCount, 
            int goodCount, 
            int badCount, 
            int missCount, 
            int totalNoteCount,
            int maxScore)
        {
            return (perfectCount * ScoreRate[Judge.Perfect] 
                   + goodCount * ScoreRate[Judge.Good] 
                   + badCount * ScoreRate[Judge.Bad] 
                   + missCount * ScoreRate[Judge.Miss])/totalNoteCount * maxScore;
        }


        private void HandleJudge(JudgeResult result)
        {
            if (result.judge is Judge.None or Judge.NotJudged) return;
            var rate = result.judge switch
            {
                Judge.Perfect => 1,
                Judge.Good => 0.7f,
                Judge.Bad => 0.4f,
                Judge.Miss => 0
            };
            switch (result.judge)
            {
                case Judge.Perfect:
                    perfectCount++;
                    break;
                case Judge.Good:
                    goodCount++;
                    break;
                case Judge.Bad:
                    badCount++;
                    break;
                case Judge.Miss:
                    missCount++;
                    break;
            }
            _internalScore = CalcScore(perfectCount, goodCount, badCount, missCount, _chartRepository.GetNoteCount(), _playSettings.maxScore);
            Score.Value = (int)_internalScore;
        }
    }
}
