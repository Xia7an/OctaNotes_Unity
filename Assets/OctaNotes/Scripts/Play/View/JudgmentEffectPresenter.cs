using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Judgment;
using OctaNotes.Scripts.Play.Model.Notes;
using UnityEngine;

namespace OctaNotes.Scripts.Play.View
{
    /// <summary>
    /// 判定エフェクトの表示管理
    /// </summary>
    public class JudgmentEffectPresenter : MonoBehaviour, IEffectPresenter
    {
        public void PresentJudgment(JudgmentEvent judgmentEvent)
        {
            ShowJudgmentText(judgmentEvent.Result, judgmentEvent.NotePosition, judgmentEvent.EffectTime);
            ShowParticle(judgmentEvent.NotePosition, judgmentEvent.EffectTime);
        }

        private void ShowJudgmentText(JudgmentResult result, int position, float time)
        {
            // 判定テキストの表示処理
            // TODO: 実際のテキスト表示実装
            Debug.Log($"[JudgmentEffect] {result} at lane {position}, time: {time}");
        }

        private void ShowParticle(int position, float time)
        {
            // パーティクルエフェクトの表示処理
            // TODO: 実際のパーティクル表示実装
        }

        public void HideNote(Note note)
        {
            // ノーツを非表示にする処理
            // TODO: 実際のノーツ非表示実装
        }
    }
}
