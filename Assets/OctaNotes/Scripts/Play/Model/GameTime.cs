using OctaNotes.Scripts.Play.Interface;
using UnityEngine;

namespace OctaNotes.Scripts.Play.Model
{
    public class GameTime: IGameTime
    {
        public double CurrentTime => AudioSettings.dspTime;
    }
}
