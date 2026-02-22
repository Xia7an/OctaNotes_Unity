using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Enum;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.Model.Struct;
using OctaNotes.Scripts.Play.ViewModel.Interface;
using R3;
using Zenject;

namespace OctaNotes.Scripts.Play.ViewModel
{
    public class JudgeSoundViewModel : IJudgeSoundViewModel, IInitializable, IDisposable
    {
        private readonly IInGameTimer _inGameTimer;
        private readonly ILaneOutputPort _laneOutputPort;
        private readonly CompositeDisposable _disposables = new();

        public JudgeSoundViewModel(IInGameTimer inGameTimer, ILaneOutputPort laneOutputPort)
        {
            _inGameTimer = inGameTimer;
            _laneOutputPort = laneOutputPort;
        }

        public ReactiveProperty<JudgeSound> JudgeForSound { get; } = new(new JudgeSound(){judge = Judge.NotJudged});

        public void Initialize()
        {
            _laneOutputPort.JudgeResult.Where(IsSoundTarget)
                .SubscribeAwait(async (result, ct) =>
                {
                    await WaitAndNotify(result, ct);
                }).AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
            JudgeForSound.Dispose();
        }

        private bool IsSoundTarget(JudgeResult result)
        {
            return result.judge != Judge.NotJudged;
        }

        private async UniTask WaitAndNotify(JudgeResult result, CancellationToken cancellationToken)
        {
            await UniTask.WaitUntil(() => result.effectInvokeTiming <= _inGameTimer.Time.Value, cancellationToken: cancellationToken);
            JudgeForSound.OnNext(new  JudgeSound()
            {
                judge = result.judge,
                isEx = result.isEx
            });
        }
    }
}
