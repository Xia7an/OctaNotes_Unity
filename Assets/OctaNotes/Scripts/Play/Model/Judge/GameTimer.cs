using OctaNotes.Scripts.Play.Model.Interface;
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

        public GameTimer()
        {
            this.Time = new ReactiveProperty<float>(0);
            Application.targetFrameRate = TARGET_FRAMERATE;
        }
        public void Tick()
        {
            this.Time.Value += 1.0f / TARGET_FRAMERATE;
        }
    }
}
