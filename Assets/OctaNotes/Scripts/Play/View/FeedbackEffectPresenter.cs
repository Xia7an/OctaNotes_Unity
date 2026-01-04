using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Judgment;
using UnityEngine;

namespace OctaNotes.Scripts.Play.View
{
    /// <summary>
    /// フィードバックエフェクトの表示管理
    /// </summary>
    public class FeedbackEffectPresenter : MonoBehaviour, IEffectPresenter
    {
        public void PresentJudgment(JudgmentEvent judgmentEvent)
        {
            HighlightLane(judgmentEvent.NotePosition);
        }

        private void HighlightLane(int position)
        {
            // レーンハイライト処理
            // TODO: 実際のハイライト実装
            Debug.Log($"[FeedbackEffect] Highlight lane {position}");
        }
    }
}
