using DefaultNamespace.Interface;
using R3;

namespace OctaNotes.Scripts.SongSelect.Model
{
    public class SongIndexState : ISongIndex
    {
        public ReactiveProperty<int> SelectedIndex { get; } =  new ReactiveProperty<int>();
    }
}
