using OctaNotes.Scripts.Core.Model;
using UnityEngine;

namespace OctaNotes.Scripts.Play.Model.Judgment.Processors
{
    /// <summary>
    /// Chainノーツの判定処理（状態なし・純粋）
    /// </summary>
    public class ChainNoteProcessor : NoteProcessorBase
    {
        public override NoteType[] SupportedNoteTypes => new[] { NoteType.Chain };

        public ChainNoteProcessor(TimingWindow timingWindow) : base(timingWindow)
        {
        }

        public override JudgmentOutput? ProcessPress(NoteProcessorContext context)
        {
            if (context.State.ChainNoteJudged)
            {
                return null;
            }

            float timeDiff = context.TimeDiff;

            // 早すぎる場合はNone
            if (timeDiff < -TimingWindow.GetMissThreshold())
            {
                return null;
            }

            // ウィンドウ内でボタンが押された
            if (IsWithinWindow(timeDiff))
            {
                float effectTime = CalculateEffectTime(context.CurrentTime, context.NoteTime, timeDiff);
                bool isEarly = timeDiff <= 0;  // 早入りかどうか

                Debug.Log($"[ChainNoteProcessor] Lane {context.LaneIndex}: Chain Perfect at {context.CurrentTime:F3}, effect at {effectTime:F3}");

                // チェイン状態をリセット
                var stateUpdate = new StateUpdate(
                    setChainNoteJudged: false,
                    setChainNoteFirstPressTime: -1f
                );

                return CreateOutput(
                    JudgmentResult.Perfect,
                    JudgmentType.Chain,
                    context.CurrentTime,
                    effectTime,
                    shouldAdvanceNote: true,
                    stateUpdate: stateUpdate,
                    isDelayed: isEarly  // 早入りの場合は遅延発火
                );
            }

            return null;
        }

        public override JudgmentOutput? ProcessTick(NoteProcessorContext context)
        {
            if (context.State.ChainNoteJudged)
            {
                return null;
            }

            float timeDiff = context.TimeDiff;

            // ウィンドウ内でボタンが押されているかチェック
            if (IsWithinWindow(timeDiff) && context.IsButtonPressed)
            {
                float effectTime = CalculateEffectTime(context.CurrentTime, context.NoteTime, timeDiff);
                bool isEarly = timeDiff <= 0;  // 早入りかどうか

                Debug.Log($"[ChainNoteProcessor] Lane {context.LaneIndex}: Chain Perfect (held) at {context.CurrentTime:F3}");

                // チェイン状態をリセット
                var stateUpdate = new StateUpdate(
                    setChainNoteJudged: false,
                    setChainNoteFirstPressTime: -1f
                );

                return CreateOutput(
                    JudgmentResult.Perfect,
                    JudgmentType.Chain,
                    context.CurrentTime,
                    effectTime,
                    shouldAdvanceNote: true,
                    stateUpdate: stateUpdate,
                    isDelayed: isEarly  // 早入りの場合は遅延発火
                );
            }

            // ウィンドウを過ぎたらMiss
            if (timeDiff > TimingWindow.GetMissThreshold())
            {
                Debug.Log($"[ChainNoteProcessor] Lane {context.LaneIndex}: Chain Miss - not pressed in window");

                // チェイン状態をリセット
                var stateUpdate = new StateUpdate(
                    setChainNoteJudged: false,
                    setChainNoteFirstPressTime: -1f
                );

                return CreateOutput(
                    JudgmentResult.Miss,
                    JudgmentType.Chain,
                    context.CurrentTime,
                    context.CurrentTime,
                    shouldAdvanceNote: true,
                    stateUpdate: stateUpdate
                );
            }

            return null;
        }
    }
}

