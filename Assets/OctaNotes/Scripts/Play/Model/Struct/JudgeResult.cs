using System;
using OctaNotes.Scripts.Play.Model.Enum;

namespace OctaNotes.Scripts.Play.Model.Struct
{
    public struct JudgeResult
    {
        public Judge judge;
        public TimingDiff timingDiff; // Fast , Just , Late (Good以下のみ)
        public int laneNumber;
        public Guid guid;
        public float originalNoteTiming; // ノーツが本来判定されるべき時刻
    }
}
