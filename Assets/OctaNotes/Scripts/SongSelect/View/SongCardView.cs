using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OctaNotes.Scripts.SongSelect.View
{
    public class SongCardView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI songNameText;
        [SerializeField] private TextMeshProUGUI songComposerText;
        [SerializeField] private TextMeshProUGUI dualLevelText;
        [SerializeField] private TextMeshProUGUI quadLevelText;
        [SerializeField] private TextMeshProUGUI octaLevelText;
        [SerializeField] private Image jacketImage;

        public void Apply(string songName, string songComposerName, float dualLevel, float quadLevel, float octaLevel, Sprite jacket)
        {
            songNameText.text = songName;
            songComposerText.text = songComposerName;
            dualLevelText.text = ((int)dualLevel).ToString();
            quadLevelText.text = ((int)quadLevel).ToString();
            octaLevelText.text = ((int)octaLevel).ToString();
            jacketImage.sprite = jacket;
        }
    }
}
