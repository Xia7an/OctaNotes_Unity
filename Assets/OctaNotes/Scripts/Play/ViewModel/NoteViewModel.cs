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
    public class NoteViewModel : INoteViewModel, IInitializable
    {
        private PlaySettingsSO _playSettingsSO;
        private IInGameTimer _inGameTimer;
        private ILaneOutputPort _laneOutputPort;
        
        public ReactiveProperty<Color> Color { get; } = new();
        public event Action OnJudged;
        public ReactiveProperty<double> PosZ { get; } = new();
        
        private double _initialPosZ = 0;
        private Guid guid = Guid.Empty;
        private CompositeDisposable _disposables = new();
        
        public NoteViewModel(PlaySettingsSO playSettingsSO, IInGameTimer inGameTimer,  ILaneOutputPort laneOutputPort)
        {
            this._playSettingsSO = playSettingsSO;
            this._inGameTimer = inGameTimer;
            this._laneOutputPort = laneOutputPort;
        }
        
        public void SetInitialPosZ(double posZ)
        {
            _initialPosZ = posZ;
        }

        public void SetGuid(Guid guid)
        {
            this.guid = guid;
        }
        

        public void Initialize()
        {
            _inGameTimer.Time.Subscribe(time =>
            {
                PosZ.Value = -time * _playSettingsSO.noteSpeed + _initialPosZ;
            }).AddTo(_disposables);
            
            // 判定が確定したノーツのGuidが自身のGuidと一致したら、エフェクト発動をスケジュール
            _laneOutputPort.JudgeResult.Where(v => 
                (
                    guid != Guid.Empty &&
                    v.guid == guid
                    && v.judge is not (Judge.NotJudged or Judge.None)
                )
            ).SubscribeAwait(async (result, ct) =>
                await ScheduleDeleteNote(result.effectInvokeTiming, ct)).AddTo(_disposables);
        }

        private async UniTask ScheduleDeleteNote(float time, CancellationToken token)
        {
            // エフェクト発動時刻まで待つ
            await UniTask.WaitUntil(() => time <= _inGameTimer.Time.Value, cancellationToken: token);
            Debug.Log($"Delete note. Guid: {guid}");
            OnJudged?.Invoke();
        }
    }
}
