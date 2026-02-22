using System;
using OctaNotes.Scripts.Play.Model.Enum;

namespace OctaNotes.Scripts.Play.Model.Struct
{
    
    public struct JudgeResult
    {
        public Judge judge;
        public bool isEx;
        public TimingDiff timingDiff; // Fast , Just , Late (Good以下のみ)
        public int laneNumber;
        public Guid guid;
        public float effectInvokeTiming; // 判定エフェクト(文字の出現、ノーツの消失)が発生する時刻
    }
}
