using System;
using DefaultNamespace;
using OctaNotes.Scripts.Core.Model;

namespace OctaNotes.Scripts.SongSelect.Model.Structs
{
    public record UIState()
    {
        public Target controlTarget;
        public Guid selectedSongId;
        public Difficulty selectedDifficulty;
        public Category selectedCategory;
        public Options selectedOption;
        public SongSort selectedSongSort;

        public float noteSpeed;
        public float judgeOffset;

    }
}
