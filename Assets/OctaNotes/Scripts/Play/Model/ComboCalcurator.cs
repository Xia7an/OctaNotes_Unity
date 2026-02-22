using System;
using System.Collections.Generic;
using OctaNotes.Scripts.Play.DI.Lane;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Enum;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.Model.Struct;
using R3;
using Zenject;

namespace OctaNotes.Scripts.Play.Model
{
    public class ComboCalcurator : IComboCalcurator, IInitializable,  IDisposable
    {
        private readonly ILaneSubContainerFactory _laneSubContainerFactory;

        public ReactiveProperty<int> Combo { get; } = new(0);

        private readonly List<ILaneOutputPort> _laneOutputPorts = new(8);
        private readonly CompositeDisposable _disposables = new();

        public ComboCalcurator(ILaneSubContainerFactory laneSubContainerFactory)
        {
            _laneSubContainerFactory = laneSubContainerFactory;
        }
        
        public void Initialize()
        {
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

        private void HandleJudge(JudgeResult result)
        {
            switch (result.judge)
            {
                case Judge.Perfect or Judge.Good or Judge.Bad:
                    Combo.Value++;
                    break;
                case Judge.Miss:
                    Combo.Value = 0;
                    break;
            }
        }
        
        public void Dispose()
        {
            _disposables?.Dispose();
            Combo?.Dispose();
        }
    }
}
