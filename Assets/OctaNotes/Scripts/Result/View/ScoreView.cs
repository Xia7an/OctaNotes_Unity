using System.Runtime.CompilerServices.Core.View.UI;
using OctaNotes.Scripts.Core.Model.Interface;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Settings;
using OctaNotes.Scripts.Utils;
using TMPro;
using UnityEngine;
using Zenject;

namespace System.Runtime.CompilerServices.Result.View
{
    public class ScoreView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        
        private IGlobalPlayResultContext  _globalPlayResultContext;
        private PlaySettingsSO _playSettingsSO;

        [Inject]
        private void Construct(IGlobalPlayResultContext globalPlayResultContext, PlaySettingsSO playSettingsSo)
        {
            _globalPlayResultContext = globalPlayResultContext;
            _playSettingsSO = playSettingsSo;
        }

        private void Start()
        {
            SetScore(_globalPlayResultContext.Score);
        }

        private void SetScore(int score)
        {
            scoreText.text = score.ToString("N0");
            if (score == _playSettingsSO.maxScore)
            {
                scoreText.SetGradientColor(
                    ColorUtils.GetColor("#FF0000"),
                    ColorUtils.GetColor("#80FF00"),
                    ColorUtils.GetColor("#00FFFF"),
                    ColorUtils.GetColor("#8000FF")
                );
            }
            else if (score >= _playSettingsSO.maxScore * 0.9)
            {
                scoreText.SetGradientColor(
                    ColorUtils.GetColor("#F5F19C"),
                    ColorUtils.GetColor("#C8B187")
                );
            }
            else if (score >= _playSettingsSO.maxScore * 0.8)
            {
                scoreText.SetGradientColor(
                    ColorUtils.GetColor("#FDCA27"),
                    ColorUtils.GetColor("#676647")
                );
            }
            else if (score >= _playSettingsSO.maxScore * 0.7)
            {
                scoreText.SetGradientColor(
                    ColorUtils.GetColor("#BCBCBC"),
                    ColorUtils.GetColor("#2B2B2B")
                );
            }
            else if (score >= _playSettingsSO.maxScore * 0.6)
            {
                scoreText.SetGradientColor(
                    ColorUtils.GetColor("#B28032"),
                    ColorUtils.GetColor("#3C0A0A")
                );
            }
            else
            {
                scoreText.SetGradientColor(
                    ColorUtils.GetColor("#656565"),
                    ColorUtils.GetColor("#000000")
                );
            }
        }
    }
}
