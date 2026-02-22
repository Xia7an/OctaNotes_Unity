using System;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Enum;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.Model.Struct;
using R3;
using Zenject;

namespace OctaNotes.Scripts.Play.DI.Lane
{
    public class LaneOutputPort : ILaneOutputPort, IInitializable, IDisposable
    {
        private readonly IJudgeContext _judgeContext;
        private readonly CompositeDisposable _disposables = new();

        public LaneOutputPort(IJudgeContext judgeContext)
        {
            _judgeContext = judgeContext;
        }
        public void Dispose()
        {
            _disposables.Dispose();
        }

        public ReactiveProperty<JudgeResult> JudgeResult { get; private set; } = new(new JudgeResult
        {
            judge = Judge.NotJudged,
            guid = Guid.Empty
        });

        public void Initialize()
        {
            _judgeContext.JudgeResult
                .Subscribe(result => JudgeResult.OnNext(result))
                .AddTo(_disposables);
        }
    }
}
