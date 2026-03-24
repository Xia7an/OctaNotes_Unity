using System;
using System.Runtime.CompilerServices.Core.ViewModel.Interfaces;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace System.Runtime.CompilerServices.Core.View
{
    public class SceneTransitView : MonoBehaviour
    {
        [SerializeField] private Image image;
        
        private ISceneTransitViewModel _viewModel;
        private Material _materialInstance;

        [Inject]
        private void Construct(ISceneTransitViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            
            // マテリアルをインスタンス化して共有マテリアルへの書き込みを防ぐ
            _materialInstance = Instantiate(image.material);
            image.material = _materialInstance;
            
            _viewModel.OnSceneLoadBegin += OnSceneLoadBegin;
            _viewModel.OnSceneLoadEnd += OnSceneLoadEnd;
        }

        private void OnDestroy()
        {
            _viewModel.OnSceneLoadBegin -= OnSceneLoadBegin;
            _viewModel.OnSceneLoadEnd -= OnSceneLoadEnd;
            
            if (_materialInstance != null)
                Destroy(_materialInstance);
        }

        private async UniTask OnSceneLoadBegin()
        {
            var tween = _materialInstance.DOFloat(1, "_Progress", 0.75f);
            if (tween == null)
            {
                Debug.LogWarning("SceneTransitView.OnSceneLoadBegin: DOFloat returned null. Check that the material has a '_Progress' shader property.");
                return;
            }
            await tween.SetEase(Ease.OutExpo).ToUniTask(cancellationToken: destroyCancellationToken);
        }
        
        private async UniTask OnSceneLoadEnd()
        {
            var tween = _materialInstance.DOFloat(-0.32f, "_Progress", 0.75f);
            if (tween == null)
            {
                Debug.LogWarning("SceneTransitView.OnSceneLoadEnd: DOFloat returned null. Check that the material has a '_Progress' shader property.");
                return;
            }
            await tween.SetEase(Ease.OutExpo).ToUniTask(cancellationToken: destroyCancellationToken);
        }
        
    }
}
