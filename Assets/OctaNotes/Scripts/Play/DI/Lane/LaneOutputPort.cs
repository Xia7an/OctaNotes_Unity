using System;
using System.Threading;
using Cysharp.Threading.Tasks;
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
        private readonly IInGameTimer _inGameTimer;
        private readonly CompositeDisposable _disposables = new();

        public LaneOutputPort(IJudgeContext judgeContext, IInGameTimer inGameTimer)
        {
            _judgeContext = judgeContext;
            _inGameTimer = inGameTimer;
        }
        public void Dispose()
        {
            _disposables.Dispose();
        }

        public ReactiveProperty<JudgeResult> JudgeResult { get; private set; } = new();

        public void Initialize()
        {
            _judgeContext.JudgeResult
                .SubscribeAwait(async (result,ct) => await AwaitJudgeUpdate(result, ct))
                .AddTo(_disposables);
        }

        // エフェクト発動時間まで待ってから判定結果を更新する
        private async UniTask AwaitJudgeUpdate(JudgeResult judgeResult, CancellationToken token)
        {
            await UniTask.WaitUntil(() => judgeResult.effectInvokeTiming <= _inGameTimer.Time.Value, cancellationToken: token);
            JudgeResult.OnNext(judgeResult);
        }
    }
}
