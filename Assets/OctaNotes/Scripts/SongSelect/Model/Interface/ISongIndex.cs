using R3;

namespace DefaultNamespace.Interface
{
    public interface ISongIndex
    {
        ReactiveProperty<int> SelectedIndex { get; }
    }
}
