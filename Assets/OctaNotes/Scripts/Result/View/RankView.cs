using System.Runtime.CompilerServices.Core.View.UI;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Settings;
using OctaNotes.Scripts.Utils;
using TMPro;
using UnityEngine;
using Zenject;

namespace System.Runtime.CompilerServices.Result.View
{
    public class RankView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI rankText;
        
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
            SetRank(_globalPlayResultContext.Score);
        }

        private void SetRank(int score)
        {
            if (score == _playSettingsSO.maxScore)
            {
                rankText.text = "O";
                rankText.SetGradientColor(
                    ColorUtils.GetColor("#FF0000"),
                    ColorUtils.GetColor("#80FF00"),
                    ColorUtils.GetColor("#00FFFF"),
                    ColorUtils.GetColor("#8000FF")
                );
            }
            else if (score >= _playSettingsSO.maxScore * 0.9)
            {
                var text = "";
                if (score >= _playSettingsSO.maxScore * 0.99)
                {
                    text = "SSS";
                }
                else if (score >= _playSettingsSO.maxScore * 0.98)
                {
                    text = "SS+";
                }
                else if (score >= _playSettingsSO.maxScore * 0.97)
                {
                    text = "SS";
                }
                else if (score >= _playSettingsSO.maxScore * 0.95)
                {
                    text = "S+";
                }
                else
                {
                    text = "S";
                }
                rankText.text = text;
                rankText.SetGradientColor(
                    ColorUtils.GetColor("#F5F19C"),
                    ColorUtils.GetColor("#C8B187")
                );
            }
            else if (score >= _playSettingsSO.maxScore * 0.8)
            {
                var text = "A";
                if (score >= _playSettingsSO.maxScore * 0.85)
                {
                    text = "A+";
                }
                rankText.text = text;
                rankText.SetGradientColor(
                    ColorUtils.GetColor("#FDCA27"),
                    ColorUtils.GetColor("#676647")
                );
            }
            else if (score >= _playSettingsSO.maxScore * 0.7)
            {
                var text = "B";
                if (score >= _playSettingsSO.maxScore * 0.75)
                {
                    text = "B+";
                }
                rankText.text = text;
                rankText.SetGradientColor(
                    ColorUtils.GetColor("#BCBCBC"),
                    ColorUtils.GetColor("#2B2B2B")
                );
            }
            else if (score >= _playSettingsSO.maxScore * 0.6)
            {
                var text = "B";
                if (score >= _playSettingsSO.maxScore * 0.65)
                {
                    text = "B+";
                }
                rankText.text = text;
                rankText.SetGradientColor(
                    ColorUtils.GetColor("#B28032"),
                    ColorUtils.GetColor("#3C0A0A")
                );
            }
            else
            {
                rankText.SetGradientColor(
                    ColorUtils.GetColor("#656565"),
                    ColorUtils.GetColor("#000000")
                );
            }
        }
    }
}
