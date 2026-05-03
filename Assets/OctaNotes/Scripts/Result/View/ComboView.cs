using System.Runtime.CompilerServices.Core.View.UI;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.Model.Struct;
using OctaNotes.Scripts.Utils;
using TMPro;
using UnityEngine;
using Zenject;

namespace System.Runtime.CompilerServices.Result.View
{
    public class ComboView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI comboText;
        
        private IGlobalPlayResultContext _globalPlayResultContext;

        [Inject]
        public void Construct(IGlobalPlayResultContext globalPlayResultContext)
        {
            _globalPlayResultContext = globalPlayResultContext;
        }

        private void Start()
        {
            SetCombo(_globalPlayResultContext.MaxCombo, _globalPlayResultContext.ClearMark);
        }

        private void SetCombo(int combo, ClearMark clearMark)
        {
            comboText.text = combo.ToString();
            var color = clearMark switch
            {
                // ClearMark.Ap     => (ColorUtils.GetColor("#FFED00"), ColorUtils.GetColor("#003BFF")),
                ClearMark.Fc     => (ColorUtils.GetColor("#B4BAF1"), ColorUtils.GetColor("#11297B")),
                ClearMark.Clear  => (ColorUtils.GetColor("#FFFFFF"), ColorUtils.GetColor("#3A3A3A")),
                ClearMark.Failed => (ColorUtils.GetColor("#3F3030"), ColorUtils.GetColor("#000000")),
                _ => throw new ArgumentOutOfRangeException(nameof(clearMark), clearMark, null)
            };
            
            comboText.SetGradientColor(color.Item1, color.Item2);
        }
    }
}
