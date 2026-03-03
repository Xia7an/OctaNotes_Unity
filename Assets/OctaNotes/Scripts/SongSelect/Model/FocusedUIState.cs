using DefaultNamespace.Interface;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Struct;
using R3;
using Zenject;

namespace DefaultNamespace
{
    public class FocusedUIState : IFocusedUI, IInitializable
    {
        private IPlayInputLayer _playInputLayer;
        
        public ReactiveProperty<SongSelectScreens> CurrentScreen { get; }
        
        private CompositeDisposable _disposables = new CompositeDisposable();

        public FocusedUIState(IPlayInputLayer playInputLayer)
        {
            _playInputLayer = playInputLayer;
        }

        public void Initialize()
        {
            _playInputLayer.IsButtonPressing[0]
                .Where(v => v == ButtonState.BeginPush && CurrentScreen.Value == SongSelectScreens.SongList)
                .Subscribe(_ => CurrentScreen.Value = SongSelectScreens.CategorySelect).AddTo(_disposables);
            
            _playInputLayer.IsButtonPressing[1]
                .Where(v => v == ButtonState.BeginPush && CurrentScreen.Value == SongSelectScreens.SongList)
                .Subscribe(_ => CurrentScreen.Value = SongSelectScreens.CategorySelect).AddTo(_disposables);
        }
        
    }
}
