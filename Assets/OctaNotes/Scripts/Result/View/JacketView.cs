using System.IO;
using OctaNotes.Scripts.Core.Model;
using OctaNotes.Scripts.Play.Model.Interface;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace OctaNotes.Scripts.Result.View
{
    public class JacketView : MonoBehaviour
    {
        [SerializeField] private Image jacket;
        [SerializeField] private Image jacketBg;
        
        private IGlobalPlayResultContext _globalPlayResultContext;
        
        [Inject]
        public void Construct(IGlobalPlayResultContext globalPlayResultContext)
        {
            _globalPlayResultContext = globalPlayResultContext;
        }

        private void Start()
        {
            SetJacket(_globalPlayResultContext.SongData.jacketPath, _globalPlayResultContext.Difficulty);
        }

        private void SetJacket(string jacketpath, Difficulty difficulty)
        {
            var jacketSprite = CreateJacketSpriteFromPath(jacketpath);
            jacket.sprite = jacketSprite;
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
            
            jacketBg.color = difficulty switch
            {
                Difficulty.Dual => new Color(124 / 255f, 168 / 255f, 221 / 255f),
                Difficulty.Quad => new Color(244 / 255f, 161 / 255f, 45 / 255f),
                Difficulty.Octa => new Color(244 / 255f, 45 / 255f, 45 / 255f)
            };
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
