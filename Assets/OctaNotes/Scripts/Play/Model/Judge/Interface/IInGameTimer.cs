using System;
using R3;

namespace OctaNotes.Scripts.Play.Model.Interface
{
    /// <summary>
    /// ゲーム内時刻をReactivePropertyで提供する
    /// 実装次第で常に任意の時刻を設定したり、倍速したりすることもできるようにしている。
    /// </summary>
    public interface IInGameTimer
    {
        public ReactiveProperty<float> Time { get; }

        public event Action OnMusicStart;

        /// <summary>
        /// タイマーが初期化され、オーディオを再生開始すべきDSP時刻が確定したときに発火する。
        /// 引数には AudioSettings.dspTime 基準での再生開始時刻が渡される。
        /// </summary>
        public event Action<double> OnTimerInitialized;
    }
}
