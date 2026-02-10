using R3;

namespace OctaNotes.Scripts.Title.Interface
{
    /// <summary>
    /// 時刻変更イベント
    /// </summary>
    public readonly struct TimeChangedEvent
    {
        public readonly double PreviousTime;
        public readonly double NewTime;

        public TimeChangedEvent(double previousTime, double newTime)
        {
            PreviousTime = previousTime;
            NewTime = newTime;
        }
    }

    /// <summary>
    /// 時刻の書き換えが可能なゲーム時刻インターフェース
    /// </summary>
    public interface ISeekableGameTime : Play.Interface.IGameTime
    {
        /// <summary>
        /// 時刻を設定する
        /// </summary>
        void SetTime(double time);

        /// <summary>
        /// 時刻変更時に発火するイベント
        /// </summary>
        Observable<TimeChangedEvent> OnTimeChanged { get; }
    }
}
