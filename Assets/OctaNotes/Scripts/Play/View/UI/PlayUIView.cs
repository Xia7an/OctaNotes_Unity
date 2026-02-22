using OctaNotes.Scripts.Play.ViewModel.Interface;
using TMPro;
using UnityEngine;
using Zenject;
using R3;

namespace OctaNotes.Scripts.Play.View.UI
{
    public class PlayUIView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI comboText;
        [SerializeField] private TextMeshProUGUI scoreText;
        
        private IPlayUIViewModel _viewModel;

        [Inject]
        private void Construct(IPlayUIViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        private void Start()
        {
            _viewModel.ComboText.Subscribe(v => comboText.text = v).AddTo(this);
            _viewModel.ScoreText.Subscribe(v => scoreText.text = v).AddTo(this);
        }
    }
}
