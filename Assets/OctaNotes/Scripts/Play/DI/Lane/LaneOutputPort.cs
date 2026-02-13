using System;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.Model.Struct;
using R3;
using Zenject;

namespace OctaNotes.Scripts.Play.DI.Lane
{
    public class LaneOutputPort : ILaneOutputPort, IInitializable, IDisposable
    {
        private readonly IJudgeContext _judgeContext;
        private readonly Subject<JudgeResult> _judgeResult = new();
        private readonly CompositeDisposable _disposables = new();

        public LaneOutputPort(IJudgeContext judgeContext)
        {
            _judgeContext = judgeContext;
        }

        public Observable<JudgeResult> OnJudgeResult => _judgeResult;

        public void Initialize()
        {
            _judgeContext.JudgeResult
                .Subscribe(result => _judgeResult.OnNext(result))
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
            _judgeResult.Dispose();
        }
    }
}
