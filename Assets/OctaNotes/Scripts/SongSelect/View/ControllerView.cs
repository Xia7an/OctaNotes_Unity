using OctaNotes.Scripts.SongSelect.Model.Structs;
using OctaNotes.Scripts.SongSelect.ViewModel.Interface;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace OctaNotes.Scripts.SongSelect.View
{
    public class ControllerView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;   
        [SerializeField] private Image background;
        
        private IHudViewModel _viewModel;
        
        [Inject]
        private void Construct(IHudViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        private void Start()
        {
            _viewModel.Param.Subscribe(SetParam).AddTo(this);
        }

        private void SetParam(HudParam param)
        {
            label.text = param.hudText;
            background.color = GetColor(param.color);
        }

        Color GetColor(HudColor color)
        {
            return color switch
            {
                HudColor.Green =>  new Color(144/255f,222/255f,102/255f),
                HudColor.Orange => new Color(244/255f,161/255f,45 /255f),
                HudColor.Yellow => new Color(216/255f,229/255f,97 /255f),
                HudColor.Cyan =>   new Color(124/255f,168/255f,222/255f),
                HudColor.Purple => new Color(224/255f,36 /255f,206/255f),
                HudColor.Black =>  new Color(47 /255f,47 /255f,47 /255f),
                HudColor.Gray =>   new Color(160/255f,160/255f,160/255f),
            };
        }
    }
}
