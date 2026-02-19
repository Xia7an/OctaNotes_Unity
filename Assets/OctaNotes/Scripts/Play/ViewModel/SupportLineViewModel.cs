using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Enum;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Settings;
using R3;
using UnityEngine;
using Zenject;
using Color = System.Drawing.Color;

namespace OctaNotes.Scripts.Play.ViewModel
{
    public class SupportLineViewModel : MonoBehaviour, ISupportLineViewModel
    {
        private PlaySettingsSO _playSettingsSO;
        private IInGameTimer _inGameTimer;
        private ILaneOutputPort _laneOutputPort;
        
        private double _initialPosZ = 0;
        public double PosZ { get; private set; }
        private Guid[]  _guids;
        
        public void SetInitialPosZ(double posZ)
        {
            _initialPosZ = posZ;
        }

        public void SetGuids(Guid[] guids)
        {
            _guids = guids;
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
                    _guids.Contains(v.guid)
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
