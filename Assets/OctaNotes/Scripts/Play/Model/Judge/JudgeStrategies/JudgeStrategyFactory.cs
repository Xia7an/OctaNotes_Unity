using OctaNotes.Scripts.Core.Model;
using OctaNotes.Scripts.Play.Model.Interface;

namespace OctaNotes.Scripts.Play.Model.JudgeStrategies
{
    public class JudgeStrategyFactory : IJudgeStrategyFactory
    {
        public IJudgeStrategy Create(NoteType noteType, bool isEx)
        { 
            IJudgeStrategy strategy = noteType switch
            {
               (NoteType.Tap or NoteType.LongStart) when !isEx => new TapJudgeStrategy(), // Exでない場合は通常の判定
               (NoteType.Tap or NoteType.LongStart) when (isEx) => new ExTapJudgeStrategy(),
               NoteType.Chain => new ChainJudgeStrategy(),
               NoteType.LongEnd => new LongEndJudgeStrategy(),
            };
            return strategy;
        }
    }
}
