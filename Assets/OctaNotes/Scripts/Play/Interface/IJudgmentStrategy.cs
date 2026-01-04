using OctaNotes.Scripts.Play.Model.Judgment;
using OctaNotes.Scripts.Play.Model.Notes;

namespace OctaNotes.Scripts.Play.Interface
{
    /// <summary>
    /// 判定戦略のインターフェース
    /// </summary>
    public interface IJudgmentStrategy
    {
        /// <summary>
        /// ボタン押下時の判定を評価する
        /// </summary>
        /// <param name="note">対象のノーツ</param>
        /// <param name="inputEvent">ボタン入力イベント</param>
        /// <returns>判定イベント（判定されなかった場合はnull）</returns>
        JudgmentEvent? EvaluateButtonPress(Note note, ButtonInputEvent inputEvent);

        /// <summary>
        /// 時間経過による判定を評価する
        /// </summary>
        /// <param name="note">対象のノーツ</param>
        /// <param name="currentTime">現在時刻</param>
        /// <returns>判定イベント（判定されなかった場合はnull）</returns>
        JudgmentEvent? EvaluateTimeProgress(Note note, float currentTime);

        /// <summary>
        /// ホールド状態が必要かどうか
        /// </summary>
        bool RequiresHoldState();
    }
}
