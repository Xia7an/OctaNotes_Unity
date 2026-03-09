using System.Collections.Generic;
using OctaNotes.Scripts.SongSelect.Model.Structs;
using R3;

namespace System.Runtime.CompilerServices.SongSelect.ViewModel.Interface
{
    public interface ISongListViewModel
    {
        ReactiveProperty<List<SongData>> SortedSongList { get; }

        event Action OnSongCursorUp;
        event Action OnSongCursorDown;
    }
}
