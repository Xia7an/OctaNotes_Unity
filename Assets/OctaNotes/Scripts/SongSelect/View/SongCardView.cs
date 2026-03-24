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

            // アスペクト比を維持しつつ 300x220 の矩形領域を覆い尽くすようにサイズを設定する
            const float targetWidth = 300f;
            const float targetHeight = 220f;

            float spriteWidth = jacket.rect.width;
            float spriteHeight = jacket.rect.height;

            float scaleByWidth = targetWidth / spriteWidth;
            float scaleByHeight = targetHeight / spriteHeight;

            // 領域全体を覆うには、より大きいスケールを採用する
            float scale = Mathf.Max(scaleByWidth, scaleByHeight);

            RectTransform rt = jacketImage.rectTransform;
            rt.sizeDelta = new Vector2(spriteWidth * scale, spriteHeight * scale);
        }
    }
}
