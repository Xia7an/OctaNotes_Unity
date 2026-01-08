using OctaNotes.Scripts.Core.Model;

namespace OctaNotes.Scripts.Play.Model.Judgment.Processors
{
    /// <summary>
    /// Tapノーツの判定処理（状態なし・純粋）
    /// </summary>
    public class TapNoteProcessor : NoteProcessorBase
    {
        public override NoteType[] SupportedNoteTypes => new[] { NoteType.Tap };

        public TapNoteProcessor(TimingWindow timingWindow) : base(timingWindow)
        {
        }

        public override JudgmentOutput? ProcessPress(NoteProcessorContext context)
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

            return CreateOutput(
                result,
                JudgmentType.Tap,
                context.CurrentTime,
                context.CurrentTime
            );
        }

        public override JudgmentOutput? ProcessTick(NoteProcessorContext context)
        {
            float timeDiff = context.TimeDiff;

            // ウィンドウを過ぎたらMiss
            if (timeDiff > TimingWindow.GetMissThreshold())
            {
                return CreateOutput(
                    JudgmentResult.Miss,
                    JudgmentType.Tap,
                    context.CurrentTime,
                    context.CurrentTime
                );
            }

            return null;
        }
    }
}
