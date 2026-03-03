using R3;

namespace DefaultNamespace.Interface
{
    public interface IFocusedUI
    {
        ReactiveProperty<SongSelectScreens> CurrentScreen { get; }
    }
}
