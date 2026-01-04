using OctaNotes.Scripts.Play.Interface;

namespace OctaNotes.Scripts.Play.Model.Notes
{
    /// <summary>
    /// ノーツの抽象基底クラス
    /// </summary>
    public abstract class Note
    {
        public float CorrectTime { get; }
        public int LanePosition { get; }
        public bool IsProcessed { get; set; }

        protected Note(float correctTime, int lanePosition)
        {
            CorrectTime = correctTime;
            LanePosition = lanePosition;
            IsProcessed = false;
        }

        /// <summary>
        /// このノーツの判定タイプを取得する
        /// </summary>
        public abstract Judgment.JudgmentType GetJudgmentType();

        /// <summary>
        /// このノーツ用の判定戦略を作成する
        /// </summary>
        public abstract IJudgmentStrategy CreateJudgmentStrategy(Judgment.TimingWindow timingWindow);
    }
}
