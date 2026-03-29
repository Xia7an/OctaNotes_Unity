using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Enum;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.Model.Struct;
using OctaNotes.Scripts.Play.ViewModel.Interface;
using OctaNotes.Scripts.Settings;
using R3;
using Zenject;

namespace OctaNotes.Scripts.Play.ViewModel
{
    public class SupportLineViewModel : ISupportLineViewModel, IInitializable, IDisposable
    {
        private PlaySettingsSO _playSettingsSO;
        private IInGameTimer _inGameTimer;
        private ILaneOutputPort _laneOutputPort;
        
        public ReactiveProperty<double> PosZ { get; } = new();
        public event Action OnJudged;
        
        private readonly HashSet<Guid> _guidSet = new();
        private double _initialPosZ = 0;
        private CompositeDisposable _disposables = new();
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private bool _isJudged;
        
        public void SetInitialPosZ(double posZ)
        {
            _initialPosZ = posZ;
        }

        public void SetGuids(Guid[] guids)
        {
            _guidSet.Clear();
            if (guids == null)
            {
                return;
            }

            for (var i = 0; i < guids.Length; i++)
            {
                var currentGuid = guids[i];
                if (currentGuid != Guid.Empty)
                {
                    _guidSet.Add(currentGuid);
                }
            }
        }
        
        public SupportLineViewModel(PlaySettingsSO playSettingsSO, IInGameTimer inGameTimer,  ILaneOutputPort laneOutputPort)
        {
            this._playSettingsSO = playSettingsSO;
            _inGameTimer = inGameTimer;
            _laneOutputPort = laneOutputPort;
        }

        public void Initialize()
        {
            _inGameTimer.Time.Subscribe(time =>
            {
                PosZ.Value = -time * _playSettingsSO.noteSpeed + _initialPosZ;
            }).AddTo(_disposables);

            _laneOutputPort.JudgeResult
                .Subscribe(HandleJudgeResult)
                .AddTo(_disposables);
        }
        
        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _disposables?.Dispose();
            PosZ?.Dispose();
        }

        private void HandleJudgeResult(JudgeResult result)
        {
            if (_isJudged || _guidSet.Count == 0)
            {
                return;
            }

            if (result.guid == Guid.Empty || !_guidSet.Contains(result.guid))
            {
                return;
            }

            if (result.judge is Judge.NotJudged or Judge.None)
            {
                return;
            }

            ScheduleDeleteNote(result.effectInvokeTiming, _cancellationTokenSource.Token).Forget();
        }

        private async UniTask ScheduleDeleteNote(float time, CancellationToken cancellationToken)
        {
            // エフェクト発動時刻まで待つ
            await UniTask.WaitUntil(() => time <= _inGameTimer.Time.Value, cancellationToken: cancellationToken);

            if (_isJudged)
            {
                return;
            }

            _isJudged = true;
            OnJudged?.Invoke();
        }

    }
}
