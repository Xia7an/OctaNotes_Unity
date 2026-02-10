using OctaNotes.Scripts.Core.Model;
using UnityEngine;

namespace OctaNotes.Scripts.Play.Model.Judgment.Processors
{
    /// <summary>
    /// LongStart/LongEndノーツの判定処理（状態なし・純粋）
    /// </summary>
    public class LongNoteProcessor : NoteProcessorBase
    {
        public override NoteType[] SupportedNoteTypes => new[] { NoteType.LongStart, NoteType.LongEnd };

        public LongNoteProcessor(TimingWindow timingWindow) : base(timingWindow)
        {
        }

        public override JudgmentOutput? ProcessPress(NoteProcessorContext context)
        {
            return context.NoteType switch
            {
                NoteType.LongStart => ProcessLongStartPress(context),
                _ => null // LongEndはボタン押下では判定しない
            };
        }

        public override JudgmentOutput? ProcessTick(NoteProcessorContext context)
        {
            return context.NoteType switch
            {
                NoteType.LongStart => ProcessLongStartTick(context),
                NoteType.LongEnd => ProcessLongEndTick(context),
                _ => null
            };
        }

        private JudgmentOutput? ProcessLongStartPress(NoteProcessorContext context)
        {
            float timeDiff = context.TimeDiff;

            // 早すぎる場合は判定しない
            if (timeDiff < -TimingWindow.GetMissThreshold())
            {
                return null;
            }

            JudgmentResult result = EvaluateTiming(timeDiff);
            if (result == JudgmentResult.None)
            {
                return null;
            }

            // ロングノーツ開始状態を更新
            var stateUpdate = new StateUpdate(
                setInLongNote: true,
                setLongNoteStartTime: context.CurrentTime,
                setLongNoteReleased: false
            );

            Debug.Log($"[LongNoteProcessor] Lane {context.LaneIndex}: Long note started at {context.CurrentTime:F3}");

            return CreateOutput(
                result,
                JudgmentType.LongStart,
                context.CurrentTime,
                context.CurrentTime,
                shouldAdvanceNote: true,
                stateUpdate: stateUpdate
            );
        }

        private JudgmentOutput? ProcessLongStartTick(NoteProcessorContext context)
        {
            float timeDiff = context.TimeDiff;

            // ウィンドウを過ぎたらMiss
            if (timeDiff > TimingWindow.GetMissThreshold())
            {
                return CreateOutput(
                    JudgmentResult.Miss,
                    JudgmentType.LongStart,
                    context.CurrentTime,
                    context.CurrentTime
                );
            }

            return null;
        }

        private JudgmentOutput? ProcessLongEndTick(NoteProcessorContext context)
        {
            // ロングノーツ中でない場合（始点をミスした場合など）は終点もMiss
            if (!context.State.IsInLongNote)
            {
                float timeDiff = context.TimeDiff;
                // ウィンドウを過ぎたらMiss判定
                if (timeDiff > TimingWindow.GetMissThreshold())
                {
                    Debug.Log($"[LongNoteProcessor] Lane {context.LaneIndex}: Long end Miss - not in long note");
                    return CreateOutput(
                        JudgmentResult.Miss,
                        JudgmentType.LongEnd,
                        context.CurrentTime,
                        context.CurrentTime
                    );
                }
                return null;
            }

            // 判定タイミング: (正しい時刻) - ウィンドウ
            float judgmentTime = context.NoteTime - TimingWindow.GetMissThreshold();

            // まだ判定タイミングに達していない
            if (context.CurrentTime < judgmentTime)
            {
                // ロングノーツ中にボタンが離されていたらMiss
                if (context.State.LongNoteReleased)
                {
                    Debug.Log($"[LongNoteProcessor] Lane {context.LaneIndex}: Long end Miss - released before judgment time");
                    var stateUpdate = new StateUpdate(setInLongNote: false);
                    return CreateOutput(
                        JudgmentResult.Miss,
                        JudgmentType.LongEnd,
                        context.CurrentTime,
                        context.CurrentTime,
                        shouldAdvanceNote: true,
                        stateUpdate: stateUpdate
                    );
                }
                return null;
            }

            // 判定タイミングに達した
            // ボタンが離されていなければPerfect
            if (!context.State.LongNoteReleased)
            {
                Debug.Log($"[LongNoteProcessor] Lane {context.LaneIndex}: Long end Perfect at {context.NoteTime:F3}");
                var stateUpdate = new StateUpdate(setInLongNote: false);

                // 現在時刻がnoteTimeより前なら遅延発火が必要
                bool isEarly = context.CurrentTime < context.NoteTime;

                return CreateOutput(
                    JudgmentResult.Perfect,
                    JudgmentType.LongEnd,
                    context.CurrentTime,
                    context.NoteTime,  // 演出は常に正しい時刻
                    shouldAdvanceNote: true,
                    stateUpdate: stateUpdate,
                    isDelayed: isEarly  // 早い場合は遅延発火
                );
            }
            else
            {
                // 判定タイミング前にボタンが離されていたらMiss
                Debug.Log($"[LongNoteProcessor] Lane {context.LaneIndex}: Long end Miss - released before judgment time");
                var stateUpdate = new StateUpdate(setInLongNote: false);
                return CreateOutput(
                    JudgmentResult.Miss,
                    JudgmentType.LongEnd,
                    context.CurrentTime,
                    context.CurrentTime,
                    shouldAdvanceNote: true,
                    stateUpdate: stateUpdate
                );
            }
        }
    }
}
