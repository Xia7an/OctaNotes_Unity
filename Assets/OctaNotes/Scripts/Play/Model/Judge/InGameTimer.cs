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
        
        private float _initialTime;
        private readonly int TARGET_FRAMERATE = 60;
        private bool isStarted = false;
        
        private readonly PlaySettingsSO _playSettings;

        public InGameTimer(PlaySettingsSO playSettings)
        {
            _playSettings = playSettings;
            
            _initialTime =-1*(float)_playSettings.songStartDelay - UnityEngine.Time.time;
            Application.targetFrameRate = TARGET_FRAMERATE;
        }
        public void Tick()
        {
            this.Time.Value = _initialTime + UnityEngine.Time.time;
            
            if (!isStarted && Time.Value >= 0)
            {
                OnMusicStart?.Invoke();
                isStarted = true;
            }
        }
    }
}
