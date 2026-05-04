using System.Runtime.CompilerServices.Core.View.UI;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.Model.Struct;
using OctaNotes.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace OctaNotes.Scripts.Result.View
{
    public class ClearMarkView : MonoBehaviour
    {
        [SerializeField] private Sprite NormalBG;
        [SerializeField] private Sprite FcBG;
        [SerializeField] private Sprite ApBG;

        [SerializeField] private Image clearMarkBG;
        [SerializeField] private TextMeshProUGUI clearMarkText;
        
        private IGlobalPlayResultContext _globalPlayResultContext;
        
        [Inject]
        public void Construct(IGlobalPlayResultContext globalPlayResultContext)
        {
            _globalPlayResultContext = globalPlayResultContext;
        }

        private void Start()
        {
            SetClearMark(_globalPlayResultContext.ClearMark);
        }

        private void SetClearMark(ClearMark clearMark)
        {
            clearMarkBG.sprite = clearMark switch
            {
                ClearMark.Ap => ApBG,
                ClearMark.Fc => FcBG,
                _ => NormalBG
            };
            clearMarkText.text = clearMark.ToString().ToUpper();
            var color = clearMark switch
            {
                ClearMark.Ap     => (ColorUtils.GetColor("#FFED00"), ColorUtils.GetColor("#003BFF")),
                ClearMark.Fc     => (ColorUtils.GetColor("#B4BAF1"), ColorUtils.GetColor("#11297B")),
                _  => (ColorUtils.GetColor("#FFFFFF"), ColorUtils.GetColor("#3A3A3A")),
            };
            clearMarkText.SetGradientColor(color.Item1, color.Item2);
        }
    }
}
