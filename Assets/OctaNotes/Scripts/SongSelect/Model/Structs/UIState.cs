using System;
using System.Collections.Generic;
using DefaultNamespace;
using OctaNotes.Scripts.Core.Model;

namespace OctaNotes.Scripts.SongSelect.Model.Structs
{
    public record UIState()
    {
        public Target controlTarget { get; init; }
        
        public int selectedSongIndex { get; init; }
        public List<SongData> songDataList { get; init; }
        
        public bool IsFirstSong => selectedSongIndex == 0;
        public bool IsLastSong => selectedSongIndex == songDataList.Count - 1;
        
        public Difficulty selectedDifficulty { get; init; }
        public SongSort selectedSongSort { get; init; }
        
        public Options selectedOptions { get; init; }
    }
}
