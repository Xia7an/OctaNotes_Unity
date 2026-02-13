using System;
using OctaNotes.Scripts.Play.Interface;
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
        public double PosZ { get; private set; } = new ();
        
        private double _initialPosZ = 0;
        public ReactiveProperty<Color> Color { get; } = new ReactiveProperty<Color>();
        public void SetInitialPosZ(double posZ)
        {
            _initialPosZ = posZ;
        }

        [Inject]
        public void Construct(PlaySettingsSO playSettingsSO, IInGameTimer inGameTimer)
        {
            this._playSettingsSO = playSettingsSO;
            _inGameTimer = inGameTimer;
        }

        private void Start()
        {
            _inGameTimer.Time.Subscribe(time =>
            {
                PosZ = -time * _playSettingsSO.noteSpeed + _initialPosZ;
            }).AddTo(this);
        }
        
    }
}
