using System;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Settings;
using R3;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.Play.Model
{
    /// <summary>
    /// 役割はinterfaceを参照
    /// </summary>
    public class InGameTimer: IInGameTimer, ITickable
    {
        public ReactiveProperty<float> Time { get; } = new(0);
        public event Action OnMusicStart;
        public event Action<double> OnTimerInitialized;

        private float _initialTime;
        private readonly int TARGET_FRAMERATE = 120;
        private bool isStarted = false;
        
        private const float SPEED_RATE = 1;
        
        private readonly PlaySettingsSO _playSettings;

        public InGameTimer(PlaySettingsSO playSettings)
        {
            _playSettings = playSettings;
            
            _initialTime =-1*(float)_playSettings.songStartDelay - UnityEngine.Time.time;
            Application.targetFrameRate = TARGET_FRAMERATE;
        }
        public void Tick()
        {
            this.Time.Value = _initialTime + UnityEngine.Time.time * SPEED_RATE;
            
            if (!isStarted && Time.Value >= 0)
            {
                OnMusicStart?.Invoke();
                isStarted = true;
                OnTimerInitialized?.Invoke(AudioSettings.dspTime);
            }
        }
    }
}
