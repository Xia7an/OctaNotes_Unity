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
    }
}
