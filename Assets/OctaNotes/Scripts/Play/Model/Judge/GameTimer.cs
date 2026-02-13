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
    public class GameTimer: IGameTimer, ITickable
    {
        public ReactiveProperty<float> Time { get; private set; }
        private readonly int TARGET_FRAMERATE = 60;
        
        private readonly PlaySettingsSO _playSettings;

        public GameTimer(PlaySettingsSO playSettings)
        {
            _playSettings = playSettings;
            this.Time = new ReactiveProperty<float>(-1*(float)_playSettings.songStartDelay);
            Application.targetFrameRate = TARGET_FRAMERATE;
        }
        public void Tick()
        {
            this.Time.Value += 1.0f / TARGET_FRAMERATE;
        }
    }
}
