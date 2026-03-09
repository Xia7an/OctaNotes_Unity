using System.Collections.Generic;
using System.Runtime.CompilerServices.SongSelect.ViewModel.Interface;
using OctaNotes.Scripts.SongSelect.Model.Interface;
using OctaNotes.Scripts.SongSelect.Model.Structs;
using R3;
using Zenject;

namespace System.Runtime.CompilerServices.SongSelect.ViewModel
{
    public class SongListViewModel : ISongListViewModel, IInitializable
    {
        private readonly ISongRepository songRepository;
        private readonly IUIState uiState;

        public SongListViewModel(ISongRepository songRepository, IUIState uiState)
        {
            this.songRepository = songRepository;
            this.uiState = uiState;
        }
        
        public ReactiveProperty<List<SongData>> SortedSongList { get; } = new(new());
        public event Action OnSongCursorUp;
        public event Action OnSongCursorDown;

        public void Initialize()
        {
            UpdateSongList();
            
        }

        private void UpdateSongList()
        {
            var tmpList = new List<SongData>();
            foreach (var guid in songRepository.SongIds)
            {
                tmpList.Add(songRepository.SongDataDict[guid]);
            }
            SortedSongList.Value = tmpList;
        }
    }
}
