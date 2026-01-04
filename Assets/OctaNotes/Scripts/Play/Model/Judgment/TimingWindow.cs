using System;

namespace OctaNotes.Scripts.Play.Model.Judgment
{
    /// <summary>
    /// 時間ウィンドウを抽象化したValue Object
    /// </summary>
    [Serializable]
    public readonly struct TimingWindow
    {
        private readonly float _perfectWindow;
        private readonly float _goodWindow;
        private readonly float _badWindow;

        public TimingWindow(float perfectWindow, float goodWindow, float badWindow)
        {
            _perfectWindow = perfectWindow;
            _goodWindow = goodWindow;
            _badWindow = badWindow;
        }

        /// <summary>
        /// タイミング差に基づいて判定結果を評価する
        /// </summary>
        /// <param name="timeDiff">正しいタイミングとの差（秒）</param>
        /// <returns>判定結果</returns>
        public JudgmentResult EvaluateTiming(float timeDiff)
        {
            float absTimeDiff = Math.Abs(timeDiff);

            if (absTimeDiff <= _perfectWindow)
            {
                return JudgmentResult.Perfect;
            }
            if (absTimeDiff <= _goodWindow)
            {
                return JudgmentResult.Good;
            }
            if (absTimeDiff <= _badWindow)
            {
                return JudgmentResult.Bad;
            }

            return JudgmentResult.None;
        }

        /// <summary>
        /// タイミング差がウィンドウ内かどうかを判定する
        /// </summary>
        /// <param name="timeDiff">正しいタイミングとの差（秒）</param>
        /// <returns>ウィンドウ内の場合true</returns>
        public bool IsWithinWindow(float timeDiff)
        {
            return Math.Abs(timeDiff) <= _badWindow;
        }

        /// <summary>
        /// Missと判定される閾値を取得する
        /// </summary>
        /// <returns>Miss閾値（秒）</returns>
        public float GetMissThreshold()
        {
            return _badWindow;
        }
    }
}
