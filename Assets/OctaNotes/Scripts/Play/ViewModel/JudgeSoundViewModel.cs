using System;
using System.Collections.Generic;
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
        private readonly Queue<JudgeResult> _waitingJudgeResults = new();

        public JudgeSoundViewModel(IInGameTimer inGameTimer, ILaneOutputPort laneOutputPort)
        {
            _inGameTimer = inGameTimer;
            _laneOutputPort = laneOutputPort;
        }

        public ReactiveProperty<JudgeSound> JudgeForSound { get; } = new(new JudgeSound(){judge = Judge.NotJudged});

        public void Initialize()
        {
            _laneOutputPort.JudgeResult.Where(IsSoundTarget)
                .Subscribe(result => _waitingJudgeResults.Enqueue(result)).AddTo(_disposables);
        }
        
        // UniTaskによる非同期処理では、タイミング"ちょうど"にイベントが発火される保証がないため、Tickによって毎フレームチェックする実装としている
        // (非同期処理は指定された時間"以上"待つことを保証しているだけ)
        public void Tick()
        {
            if(_waitingJudgeResults.Count == 0) return; // エフェクト発動待ちのノーツがなければ何もしない

            while (_waitingJudgeResults.Count > 0 && _waitingJudgeResults.Peek().effectInvokeTiming <= _inGameTimer.Time.Value)
            {
                var result = _waitingJudgeResults.Dequeue();

                // 時間になったらイベント発火
                JudgeForSound.OnNext(new JudgeSound()
                {
                    judge = result.judge,
                    isEx = result.isEx
                });
            }
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
