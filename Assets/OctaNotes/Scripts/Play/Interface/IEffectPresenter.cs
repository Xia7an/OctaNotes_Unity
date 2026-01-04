using OctaNotes.Scripts.Play.Model.Judgment;

namespace OctaNotes.Scripts.Play.Interface
{
    /// <summary>
    /// 演出管理のインターフェース
    /// </summary>
    public interface IEffectPresenter
    {
        /// <summary>
        /// 判定に応じた演出を表示する
        /// </summary>
        /// <param name="judgmentEvent">判定イベント</param>
        void PresentJudgment(JudgmentEvent judgmentEvent);
    }
}
