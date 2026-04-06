using System;
using OctaNotes.Scripts.Core.Model.Interface;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.Model.Struct;
using OctaNotes.Scripts.Play.ViewModel.Interface;
using OctaNotes.Scripts.Settings;
using R3;
using Zenject;

namespace OctaNotes.Scripts.Play.ViewModel
{
    public class LongNoteViewModel : ILongNoteViewModel, IInitializable
    {
        private readonly IInputLayer inputLayer;
        private readonly ILaneContext _laneContext;
        private readonly IInGameTimer _inGameTimer;
        private readonly PlaySettingsSO _playSettingsSO;

        public ReactiveProperty<double> PosZ { get; } = new();
        public ReactiveProperty<bool> IsPushed { get; } = new();
        
        private double _initialPosZ;
        private Guid[]  _guids;
        private CompositeDisposable _disposables = new();

        public LongNoteViewModel(IInputLayer inputLayer, ILaneContext laneContext, IInGameTimer inGameTimer,  PlaySettingsSO playSettingsSO)
        {
            this.inputLayer = inputLayer;
            _laneContext = laneContext;
            _inGameTimer = inGameTimer;
            _playSettingsSO = playSettingsSO;
        }
        
        public void Initialize()
        {
            // レーンに対応するボタンが押されているかどうかを反映する
            inputLayer.IsButtonPressing[_laneContext.LaneIndex]
                .Subscribe(state =>
                {
                    IsPushed.Value = state is ButtonState.BeginPush or ButtonState.Pushed;
                }).AddTo(_disposables);
            
            // 時刻に応じたノーツ座標を設定
            _inGameTimer.Time.Subscribe(time =>
            {
                PosZ.Value = -time * _playSettingsSO.noteSpeed + _initialPosZ;
            }).AddTo(_disposables);
        }
        
        public void SetInitialPosZ(double posZ)
        {
            _initialPosZ = posZ;
        }

        public void SetGuids(Guid[] guids)
        {
            _guids = guids;
        }

    }
}
