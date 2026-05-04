using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly List<(double timing, double hs)> _hsChanges;

        public ReactiveProperty<double> PosZ { get; } = new();
        public ReactiveProperty<bool> IsPushed { get; } = new();
        
        private double _initialPosZ;
        private Guid[]  _guids;
        private CompositeDisposable _disposables = new();

        public LongNoteViewModel(
            IInputLayer inputLayer,
            ILaneContext laneContext,
            IInGameTimer inGameTimer,
            PlaySettingsSO playSettingsSO,
            IChartRepositoryImmutable chartRepository)
        {
            this.inputLayer = inputLayer;
            _laneContext = laneContext;
            _inGameTimer = inGameTimer;
            _playSettingsSO = playSettingsSO;
            _hsChanges = chartRepository.HsChangeData
                .OrderBy(x => x.Item1)
                .Select(x => (x.Item1, x.Item2))
                .ToList();
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
                var traveledPosition = CalcPositionByHs(time);
                PosZ.Value = -traveledPosition * _playSettingsSO.noteSpeed + _initialPosZ;
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

        private double CalcPositionByHs(double time)
        {
            var pos = 0d;
            var currentTime = 0d;
            var currentHs = 1d;

            for (var i = 0; i < _hsChanges.Count; i++)
            {
                var (changeTime, nextHs) = _hsChanges[i];
                if (changeTime > time)
                {
                    break;
                }

                var dt = changeTime - currentTime;
                if (dt > 0d)
                {
                    pos += currentHs * dt;
                }

                currentTime = changeTime;
                currentHs = nextHs;
            }

            var remain = time - currentTime;
            pos += currentHs * remain;

            return pos;
        }

    }
}
