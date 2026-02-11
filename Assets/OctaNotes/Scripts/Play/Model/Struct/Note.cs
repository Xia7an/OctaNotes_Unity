using System;
using OctaNotes.Scripts.Core.Model;

namespace OctaNotes.Scripts.Play.Model.Struct
{
    public struct Note
    {
        public NoteType noteType;
        public Guid guid;
        public float justTiming;
        public float timingDiff;
        public int laneNumber;
    }
}
