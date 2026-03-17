using System;
using System.Runtime.CompilerServices.Core.ViewModel.Interfaces;
using Cysharp.Threading.Tasks;
using OctaNotes.Scripts.Core.Model.Interface;
using R3;
using Zenject;

namespace System.Runtime.CompilerServices.Core.ViewModel
{
    public class SceneTransitViewModel: IInitializable, ISceneTransitViewModel
    {
        private readonly ISceneController sceneController;
        
        public event Func<UniTask> OnSceneLoadBegin;
        public event Func<UniTask> OnSceneLoadEnd;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        public SceneTransitViewModel(ISceneController sceneController)
        {
            this.sceneController = sceneController;
        }

        public void Initialize()
        {
            sceneController.OnSceneLoadBegin += InvokeOnSceneLoadBegin;
            sceneController.OnSceneLoadEnd += InvokeOnSceneLoadEnd;
        }

        private async UniTask InvokeOnSceneLoadBegin()
        {
            if (OnSceneLoadBegin != null)
                await UniTask.WhenAll(Array.ConvertAll(OnSceneLoadBegin.GetInvocationList(), d => ((Func<UniTask>)d)()));
        }

        private async UniTask InvokeOnSceneLoadEnd()
        {
            if (OnSceneLoadEnd != null)
                await UniTask.WhenAll(Array.ConvertAll(OnSceneLoadEnd.GetInvocationList(), d => ((Func<UniTask>)d)()));
        }
    }
}
