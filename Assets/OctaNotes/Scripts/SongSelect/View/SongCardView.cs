using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace System.Runtime.CompilerServices.SongSelect.View
{
    public class SongCardView: MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI songNameText;
        [SerializeField] private TextMeshProUGUI songComposerText;
        [SerializeField] private TextMeshProUGUI dualLevelText;
        [SerializeField] private TextMeshProUGUI quadLevelText;
        [SerializeField] private TextMeshProUGUI octaLevelText;
        [SerializeField] private Image jacketImage;

        public void Apply(string songName, string songComposerName, int dualLevel, int quadLevel, int octaLevel, Image jacket)
        {
            songNameText.text = songName;
            songComposerText.text = songComposerName;
            dualLevelText.text = dualLevel.ToString();
            quadLevelText.text = quadLevel.ToString();
            octaLevelText.text = octaLevel.ToString();
            jacketImage = jacket;
        }
    }
}
