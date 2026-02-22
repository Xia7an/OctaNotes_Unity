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
    public class JudgeSoundViewModel : IJudgeSoundViewModel, IInitializable, IDisposable, ITickable
    {
        private readonly IInGameTimer _inGameTimer;
        private readonly ILaneOutputPort _laneOutputPort;
        private readonly CompositeDisposable _disposables = new();

        private bool _isWaiting = false;

        public JudgeSoundViewModel(IInGameTimer inGameTimer, ILaneOutputPort laneOutputPort)
        {
            _inGameTimer = inGameTimer;
            _laneOutputPort = laneOutputPort;
        }

        public ReactiveProperty<JudgeSound> JudgeForSound { get; } = new(new JudgeSound(){judge = Judge.NotJudged});

        public void Initialize()
        {
            _laneOutputPort.JudgeResult.Where(IsSoundTarget)
                .Subscribe(_ => _isWaiting = true).AddTo(_disposables);
        }
        
        // UniTaskによる非同期処理では、タイミング"ちょうど"にイベントが発火される保証がないため、Tickによって毎フレームチェックする実装としている
        // (非同期処理は指定された時間"以上"待つことを保証しているだけ)
        public void Tick()
        {
            if(!_isWaiting) return; // エフェクト発動待ちのノーツがなければ何もしない
            if (!(_laneOutputPort.JudgeResult.Value.effectInvokeTiming <= _inGameTimer.Time.Value)) return; // 時間までは何もしない
            
            // 時間になったらイベント発火 & 待ち状態解除
            JudgeForSound.OnNext(new JudgeSound()
            {
                judge = _laneOutputPort.JudgeResult.Value.judge,
                isEx = _laneOutputPort.JudgeResult.Value.isEx
            });
            _isWaiting = false;
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
        
    }
}
