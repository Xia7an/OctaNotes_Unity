using OctaNotes.Scripts.Core.Model;
using UnityEngine;

namespace OctaNotes.Scripts.Play.Model.Judgment.Processors
{
    /// <summary>
    /// Processor共通の基底クラス
    /// </summary>
    public abstract class NoteProcessorBase : INoteProcessor
    {
        protected readonly TimingWindow TimingWindow;

        protected NoteProcessorBase(TimingWindow timingWindow)
        {
            TimingWindow = timingWindow;
        }

        public abstract NoteType[] SupportedNoteTypes { get; }

        public abstract JudgmentOutput? ProcessPress(NoteProcessorContext context);

        public abstract JudgmentOutput? ProcessTick(NoteProcessorContext context);

        /// <summary>
        /// 判定結果を生成するヘルパーメソッド
        /// </summary>
        protected JudgmentOutput CreateOutput(
            JudgmentResult result,
            JudgmentType type,
            float evaluatedTime,
            float effectTime,
            bool shouldAdvanceNote = true,
            StateUpdate? stateUpdate = null,
            bool isDelayed = false)
        {
            return new JudgmentOutput(result, type, evaluatedTime, effectTime, shouldAdvanceNote, stateUpdate, isDelayed);
        }

        /// <summary>
        /// タイミング差から判定結果を評価
        /// </summary>
        protected JudgmentResult EvaluateTiming(float timeDiff)
        {
            return TimingWindow.EvaluateTiming(timeDiff);
        }

        /// <summary>
        /// タイミング差がウィンドウ内かどうかを判定
        /// </summary>
        protected bool IsWithinWindow(float timeDiff)
        {
            return TimingWindow.IsWithinWindow(timeDiff);
        }

        /// <summary>
        /// 早入りか遅入りかに基づいて演出時刻を計算
        /// </summary>
        protected float CalculateEffectTime(float currentTime, float noteTime, float timeDiff)
        {
            // 早入り: 演出は正しい時刻に発生
            // 遅入り: 演出はボタンが押された時刻に発生
            return timeDiff <= 0 ? noteTime : currentTime;
        }
    }
}
