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
            jacket.sprite = CreateJacketSpriteFromPath(jacketpath);
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
