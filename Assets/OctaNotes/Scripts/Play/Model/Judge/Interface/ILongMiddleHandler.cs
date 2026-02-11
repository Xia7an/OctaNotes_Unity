using R3;

namespace OctaNotes.Scripts.Play.Model.Interface
{
    public interface ILongMiddleHandler
    {
        /// <summary>
        /// ロング視点から終点までの何割ボタンが押されていたかを返す
        /// </summary>
        public ReactiveProperty<float> LongPushedRate { get; }
    }
}
