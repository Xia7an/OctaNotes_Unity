using UnityEngine;

namespace OctaNotes.Scripts.Settings
{
    [CreateAssetMenu(fileName = "PlaySettings", menuName = "Scriptable Objects/PlaySettings")]
    public class PlaySettingsSO : ScriptableObject
    {
        public double noteSpeed = 5.0;
    }
}
