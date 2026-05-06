using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using OctaNotes.Scripts.SongSelect.Model.Interface;
using OctaNotes.Scripts.SongSelect.Model.Structs;
using R3;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.SongSelect.View
{
    public class TargetSwitcher : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Target controlTarget;
        
        private IUIState _uiState;

        [Inject]
        private void Construct(IUIState uiState)
        {
            _uiState = uiState;
        }

        private void Start()
        {
            _uiState.State
                .Select(v => v.controlTarget)
                .SubscribeAwait(async (v, ct) =>
                    {
                        await SetUIShow(v == controlTarget, ct);
                    }
                ).AddTo(this);
        }

        private async UniTask SetUIShow(bool isShow, CancellationToken ct)
        {
            await canvasGroup.DOFade(isShow ? 1 : 0, 0.1f).ToUniTask(cancellationToken: ct);
        }
    }
}
