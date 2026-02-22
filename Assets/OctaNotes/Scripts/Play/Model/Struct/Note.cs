using System;
using OctaNotes.Scripts.Core.Model;

namespace OctaNotes.Scripts.Play.Model.Struct
{
    public struct Note
    {
        public NoteType noteType;
        public bool isEx;
        public Guid guid;
        public float justTiming;
        public float timingDelta;
        public int laneNumber;
    }
}
