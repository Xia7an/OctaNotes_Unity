using System;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.Model.Struct;
using R3;
using Zenject;
using JudgeType = OctaNotes.Scripts.Play.Model.Enum.Judge;

namespace OctaNotes.Scripts.Play.DI.Lane
{
    public class LaneViewModel : ILaneViewModel, IInitializable, IDisposable
    {
        private readonly ILaneContext _laneContext;
        private readonly IPlayInputLayer _playInputLayer;
        private readonly IJudgeContext _judgeContext;
        private readonly CompositeDisposable _disposables = new();

        public LaneViewModel(ILaneContext laneContext, IPlayInputLayer playInputLayer, IJudgeContext judgeContext)
        {
            _laneContext = laneContext;
            _playInputLayer = playInputLayer;
            _judgeContext = judgeContext;
        }

        public ReactiveProperty<ButtonState> ButtonState { get; } = new(Model.Struct.ButtonState.Released);
        public ReactiveProperty<JudgeType> CurrentJudge { get; } = new(JudgeType.NotJudged);

        public void Initialize()
        {
            _playInputLayer.IsButtonPressing[_laneContext.LaneIndex]
                .Subscribe(state => ButtonState.Value = state)
                .AddTo(_disposables);

            _judgeContext.JudgeResult
                .Subscribe(result => CurrentJudge.Value = result.judge)
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
            ButtonState.Dispose();
            CurrentJudge.Dispose();
        }
    }
}
