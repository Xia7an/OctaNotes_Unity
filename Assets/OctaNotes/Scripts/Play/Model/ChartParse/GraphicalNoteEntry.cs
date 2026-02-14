using System;
using OctaNotes.Scripts.Core.Model;

namespace OctaNotes.Scripts.Play.Model
{
    public struct GraphicalNoteEntry
    {
        public NoteType noteType;
        public NoteColor noteColor;
        public int noteColorLevel;
        public Guid  guid;
    }

    public enum NoteColor
    {
        Blue,
        Red,
        Yellow,
    }
}
