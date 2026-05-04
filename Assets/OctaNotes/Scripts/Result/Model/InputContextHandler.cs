using System;
using OctaNotes.Scripts.Core.Model.Interface;
using OctaNotes.Scripts.Core.Model.Structs;
using OctaNotes.Scripts.Play.Model.Struct;
using R3;
using Zenject;

namespace OctaNotes.Scripts.Result.Model
{
    public class InputContextHandler: IInitializable, IDisposable
    {
        private readonly IInputLayer _inputLayer;
        private readonly ISceneController _sceneController;
        
        private CompositeDisposable  _disposables = new CompositeDisposable();

        public InputContextHandler(IInputLayer inputLayer,  ISceneController sceneController)
        {
            _inputLayer = inputLayer;
            _sceneController = sceneController;
        }

        public void Initialize()
        {
            // 任意のボタンでSongSelectに遷移する
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
            _disposables.Dispose();
        }
    }
}
