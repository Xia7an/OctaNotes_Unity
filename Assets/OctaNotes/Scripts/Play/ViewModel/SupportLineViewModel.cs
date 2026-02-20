using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Enum;
using OctaNotes.Scripts.Play.Model.Interface;
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
        
        private Guid[]  _guids;
        private double _initialPosZ = 0;
        private CompositeDisposable _disposables = new();
        
        public void SetInitialPosZ(double posZ)
        {
            _initialPosZ = posZ;
        }

        public void SetGuids(Guid[] guids)
        {
            _guids = guids;
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
            _laneOutputPort.JudgeResult.Where(v => 
                (
                    _guids != null
                    && v.guid != Guid.Empty
                    &&
                    _guids.Contains(v.guid)
                    && v.judge is not (Judge.NotJudged or Judge.None)
                )
            ).SubscribeAwait(async (result, ct) =>
                await ScheduleDeleteNote(result.effectInvokeTiming, ct)).AddTo(_disposables);
        }
        
        public void Dispose()
        {
            _disposables?.Dispose();
            PosZ?.Dispose();
        }

        private async UniTask ScheduleDeleteNote(float time, CancellationToken token)
        {
            // エフェクト発動時刻まで待つ
            await UniTask.WaitUntil(() => time <= _inGameTimer.Time.Value, cancellationToken: token);
            OnJudged?.Invoke();
        }

    }
}
