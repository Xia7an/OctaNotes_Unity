using System;
using System.Collections.Generic;
using OctaNotes.Scripts.Play.Interface;
using R3;
using Zenject;

namespace OctaNotes.Scripts.Play.DI.Lane
{
    public class LaneInputManager : IInitializable, IDisposable
    {
        private readonly IPlayInputLayer _playInputLayer;
        private readonly List<ILaneInputPort> _laneInputPorts;
        private readonly CompositeDisposable _disposables = new();

        public LaneInputManager(IPlayInputLayer playInputLayer, List<ILaneInputPort> laneInputPorts)
        {
            _playInputLayer = playInputLayer;
            _laneInputPorts = laneInputPorts;
        }

        public void Initialize()
        {
            for (int i = 0; i < _laneInputPorts.Count; i++)
            {
                int laneIndex = i;
                _playInputLayer.IsButtonPressing[laneIndex]
                    .Subscribe(state => _laneInputPorts[laneIndex].UpdateButtonState(state))
                    .AddTo(_disposables);
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
