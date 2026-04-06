using System.IO;
using OctaNotes.Scripts.Core.Model;
using OctaNotes.Scripts.SongSelect.Model.Interface;
using OctaNotes.Scripts.SongSelect.Model.Structs;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace OctaNotes.Scripts.SongSelect.View
{
    public class SongInfoView : MonoBehaviour
    {
        [SerializeField] private Image jacket;
        [SerializeField] private Image jacketBg;
        [SerializeField] private TextMeshProUGUI songTitle;
        [SerializeField] private TextMeshProUGUI songComposer;
        
        private IUIState _uiState;

        [Inject]
        private void Construct(IUIState uiState)
        {
            _uiState = uiState;
        }

        private void Start()
        {
            _uiState.State.Select(v => v.selectedDifficulty).Subscribe(SetJacketBgColor).AddTo(this);
            _uiState.State.Select(v => v.selectedSongIndex)
                .Subscribe(
                    idx => UpdateSongInfo(_uiState.State.Value.songDataList[idx])
                    ).AddTo(this);
        }

        private void SetJacketBgColor(Difficulty difficulty)
        {
            jacketBg.color = difficulty switch
            {
                Difficulty.Dual => new Color(124 / 255f, 168 / 255f, 221 / 255f),
                Difficulty.Quad => new Color(244 / 255f, 161 / 255f, 45 / 255f),
                Difficulty.Octa => new Color(244 / 255f, 45 / 255f, 45 / 255f)
            };
        }

        private void UpdateSongInfo(SongData songData)
        {
            var jacketSprite = CreateJacketSpriteFromPath(songData.jacketPath);
            songTitle.text = songData.songName;
            songComposer.text = songData.composerName;
            
            // アスペクト比を維持しつつ 390x390 の矩形領域を覆い尽くすようにサイズを設定する
            const float targetWidth = 390f;
            const float targetHeight = 390f;

            float spriteWidth = jacketSprite.rect.width;
            float spriteHeight = jacketSprite.rect.height;

            float scaleByWidth = targetWidth / spriteWidth;
            float scaleByHeight = targetHeight / spriteHeight;

            // 領域全体を覆うには、より大きいスケールを採用する
            float scale = Mathf.Max(scaleByWidth, scaleByHeight);

            RectTransform rt = jacket.rectTransform;
            rt.sizeDelta = new Vector2(spriteWidth * scale, spriteHeight * scale);
            jacket.sprite = jacketSprite;
        }
        
        private Sprite CreateJacketSpriteFromPath(string jacketPath)
        {
            if (string.IsNullOrWhiteSpace(jacketPath) || !File.Exists(jacketPath))
            {
                Debug.LogWarning($"{jacketPath} not found.");
                return null;
            }

            var bytes = File.ReadAllBytes(jacketPath);
            var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!texture.LoadImage(bytes))
            {
                Destroy(texture);
                return null;
            }

            return Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f));
        }
    }
}
