using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Enum;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Settings;
using R3;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.Play.ViewModel
{
    public class NoteViewModel : MonoBehaviour, INoteViewModel
    {
        private PlaySettingsSO _playSettingsSO;
        private IInGameTimer _inGameTimer;
        private ILaneOutputPort _laneOutputPort;
        
        public double PosZ { get; private set; } = new ();
        
        private double _initialPosZ = 0;
        private Guid guid = Guid.Empty;
        
        public ReactiveProperty<Color> Color { get; } = new ReactiveProperty<Color>();
        public void SetInitialPosZ(double posZ)
        {
            _initialPosZ = posZ;
        }

        public void SetGuid(Guid guid)
        {
            this.guid = guid;
        }

        [Inject]
        public void Construct(PlaySettingsSO playSettingsSO, IInGameTimer inGameTimer,  ILaneOutputPort laneOutputPort)
        {
            this._playSettingsSO = playSettingsSO;
            _inGameTimer = inGameTimer;
            _laneOutputPort = laneOutputPort;
        }

        private void Start()
        {
            _inGameTimer.Time.Subscribe(time =>
            {
                PosZ = -time * _playSettingsSO.noteSpeed + _initialPosZ;
            }).AddTo(this);
            _laneOutputPort.JudgeResult.Where(v => 
                (
                    v.guid == guid
                    && v.judge is not (Judge.NotJudged or Judge.None)
                )
            ).SubscribeAwait(async (result, ct) =>
                await ScheduleDeleteNote(result.effectInvokeTiming, ct)).AddTo(this);
        }

        private async UniTask ScheduleDeleteNote(float time, CancellationToken token)
        {
            // エフェクト発動時刻まで待つ
            await UniTask.WaitUntil(() => time <= _inGameTimer.Time.Value, cancellationToken: token);
            Destroy(gameObject);
        }
        
    }
}
