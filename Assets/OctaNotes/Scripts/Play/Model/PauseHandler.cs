using System;
using OctaNotes.Scripts.Core.Model.Interface;
using OctaNotes.Scripts.Core.Model.Structs;
using OctaNotes.Scripts.Play.Model.Struct;
using R3;
using Zenject;

namespace OctaNotes.Scripts.Play.Model
{
    public class PauseHandler: IInitializable, IDisposable
    {
        private readonly IInputLayer _inputLayer;
        private readonly ISceneController _sceneController;
        
        private CompositeDisposable _disposables = new CompositeDisposable();

        
        public PauseHandler(IInputLayer inputLayer,  ISceneController sceneController)
        {
            _inputLayer =  inputLayer;
            _sceneController = sceneController;
        }

        public void Initialize()
        {
            _inputLayer.IsPauseButtonPressed
                .Where(v => v is ButtonState.BeginPush)
                .Subscribe(_ => _sceneController.ChangeScene(Scenes.SongSelect))
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}
