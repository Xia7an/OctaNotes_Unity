using System;
using OctaNotes.Scripts.Core.Model;
using OctaNotes.Scripts.Core.Model.Interface;
using OctaNotes.Scripts.Core.Model.Structs;
using OctaNotes.Scripts.Play.Model.Struct;
using R3;
using Zenject;

namespace OctaNotes.Scripts.Title.Model
{
    public class InputContextHandler: IInitializable, IDisposable
    {
        private readonly InputLayer _inputLayer;
        private readonly ISceneController _sceneController;
        
        private readonly CompositeDisposable  _disposables = new CompositeDisposable();
        
        public InputContextHandler(InputLayer inputLayer, ISceneController sceneController)
        {
            _inputLayer = inputLayer;
            _sceneController = sceneController;
        }
        
        public void Initialize()
        {
            for (int i = 0; i < 8; i++)
            {
                var i1 = i;
                _inputLayer.IsButtonPressing[i1]
                    .Where(v => v is ButtonState.BeginPush)
                    .SubscribeAwait((_, ct) => _sceneController.ChangeScene(Scenes.SongSelect))
                    .AddTo(_disposables);
            }
        }

        public void Dispose()
        {
            _inputLayer?.Dispose();
            _disposables?.Dispose();
        }
    }
}
