using OctaNotes.Scripts.Title.Interface;
using R3;

namespace OctaNotes.Scripts.Title.Model
{
    /// <summary>
    /// 時刻書き換え可能なゲーム時刻の実装
    /// </summary>
    public class SeekableGameTime : ISeekableGameTime
    {
        private double _currentTime;
        private readonly Subject<TimeChangedEvent> _timeChangedSubject = new Subject<TimeChangedEvent>();

        public double CurrentTime => _currentTime;

        public Observable<TimeChangedEvent> OnTimeChanged => _timeChangedSubject;

        public SeekableGameTime(double initialTime = 0.0)
        {
            _currentTime = initialTime;
        }

        /// <summary>
        /// 時刻を設定する（イベント発火あり）
        /// </summary>
        public void SetTime(double time)
        {
            if (System.Math.Abs(_currentTime - time) < 0.0001) return;

            var previousTime = _currentTime;
            _currentTime = time;
            _timeChangedSubject.OnNext(new TimeChangedEvent(previousTime, time));
        }

        /// <summary>
        /// 時刻を進める（イベント発火なし、通常のTick用）
        /// </summary>
        public void AdvanceTime(double deltaTime)
        {
            _currentTime += deltaTime;
        }

        public void Dispose()
        {
            _timeChangedSubject.Dispose();
        }
    }
}
