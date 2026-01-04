using System;
using OctaNotes.Scripts.Core.Model;
using OctaNotes.Scripts.Settings;
using Unity.Mathematics;
using Zenject;

namespace OctaNotes.Scripts.Play.Model
{
    public class JudgeEngine
    {
        [Inject] private readonly PlaySettingsSO _playSettingsSO;
        
        public Judge JudgeTap(double currentTime, double noteTime)
        {
            double delta = currentTime - noteTime;
            double absDelta = System.Math.Abs(delta);
            
            if (absDelta <= _playSettingsSO.perfectRangeMs)
            {
                return Judge.Perfect;
            }
            else if (absDelta <= _playSettingsSO.goodRangeMs)
            {
                return Judge.Good;
            }
            else if (absDelta <= _playSettingsSO.badRangeMs)
            {
                return Judge.Bad;
            }
            else if(absDelta > _playSettingsSO.maxMissDelayMs)
            {
                return Judge.Miss;   
            }
            return Judge.None;
        }
        
        public Judge JudgeLongEnd(double currentTime, double noteEndTime)
        {
            double delta = currentTime - noteEndTime;
            if (delta >= -0.15)
            {
                return Judge.Perfect;
            }
            else
            {
                return Judge.Miss;
            }
        }
    }
}
