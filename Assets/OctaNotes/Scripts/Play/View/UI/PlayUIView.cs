using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
            _viewModel.ComboText.SubscribeAwait(async (v,ct) 
                => await SetComboText(v,ct), AwaitOperation.Switch).AddTo(this);
            _viewModel.ScoreText.Subscribe(v => scoreText.text = v).AddTo(this);
        }

        private async UniTask SetComboText(string combo, CancellationToken ct)
        {
            comboText.text = combo;
            await comboText.rectTransform.DOScale(1.4f, 0.2f)
                .SetEase(Ease.OutSine).ToUniTask(cancellationToken: ct);
            await comboText.rectTransform.DOScale(1f, 0.1f)
                .SetEase(Ease.InSine).ToUniTask(cancellationToken: ct);
        }
        
    }
}
