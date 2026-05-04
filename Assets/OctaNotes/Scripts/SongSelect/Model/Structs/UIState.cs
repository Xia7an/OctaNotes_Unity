using System;
using System.Collections.Generic;
using DefaultNamespace;
using OctaNotes.Scripts.Core.Model;

namespace OctaNotes.Scripts.SongSelect.Model.Structs
{
    public record UIState()
    {
        public Target controlTarget { get; init; }

        public CursorState cursor { get; init; } = new();
        
        public int selectedSongIndex => cursor.songIndex;
        public List<SongData> songDataList { get; init; }
        
        public bool IsFirstSong => selectedSongIndex == 0;
        public bool IsLastSong => selectedSongIndex == songDataList.Count - 1;
        
        public Difficulty selectedDifficulty { get; init; }
        public SongSort selectedSongSort { get; init; }
        
        public Options selectedOptions => (Options)cursor.optionIndex;

        public double noteSpeed { get; init; }
        public int judgeOffsetMs { get; init; }

        public bool IsOptionValueAtMin => selectedOptions switch
        {
            Options.NoteSpeed => noteSpeed <= 1.0,
            Options.JudgeOffset => judgeOffsetMs <= -20,
            _ => false
        };

        public bool IsOptionValueAtMax => selectedOptions switch
        {
            Options.NoteSpeed => noteSpeed >= 20.0,
            Options.JudgeOffset => judgeOffsetMs >= 20,
            _ => false
        };
    }
}
