using UnityEngine;

namespace OctaNotes.Scripts.Settings
{
    [CreateAssetMenu(fileName = "PlaySettings", menuName = "Scriptable Objects/PlaySettings")]
    public class PlaySettingsSO : ScriptableObject
    {
        public double noteSpeed = 5.0;
        public int perfectRangeMs = 50; // ms
        public int goodRangeMs = 100;  // ms
        public int badRangeMs = 150;
        public int maxMissDelayMs = 200;
        public double songStartDelay = 3;

        public int maxScore = 1000000;
    }
}
