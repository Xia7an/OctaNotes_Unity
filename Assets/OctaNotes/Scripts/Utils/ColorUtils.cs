using UnityEngine;

namespace OctaNotes.Scripts.Utils
{
    public class ColorUtils
    {
        public static Color GetColor(string htmlCode)
        {
            UnityEngine.ColorUtility.TryParseHtmlString(htmlCode, out var color);
            return color;
        }
    }
}
