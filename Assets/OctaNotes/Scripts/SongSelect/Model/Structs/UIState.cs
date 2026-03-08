using System;
using System.Collections.Generic;
using DefaultNamespace;
using OctaNotes.Scripts.Core.Model;

namespace OctaNotes.Scripts.SongSelect.Model.Structs
{
    public record UIState()
    {
        public Target controlTarget;
        
        public Guid selectedSongId;
        public Dictionary<Guid, GuidBridge> guidList; // Keyとして与えられたGuidの次と前のGuidを保持するDict 
        
        public Difficulty selectedDifficulty;
        public Category selectedCategory;
        public Options selectedOption;
        public SongSort selectedSongSort;

        public float noteSpeed;
        public float judgeOffset;

        public bool songConfirmed;
    }
}
