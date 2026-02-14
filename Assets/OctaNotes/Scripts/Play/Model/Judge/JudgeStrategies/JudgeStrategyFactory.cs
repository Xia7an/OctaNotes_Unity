using OctaNotes.Scripts.Core.Model;
using OctaNotes.Scripts.Play.Model.Interface;

namespace OctaNotes.Scripts.Play.Model.JudgeStrategies
{
    public class JudgeStrategyFactory : IJudgeStrategyFactory
    {
        public IJudgeStrategy Create(NoteType noteType)
        { 
            IJudgeStrategy strategy = noteType switch
            {
               NoteType.Tap or NoteType.LongStart => new TapJudgeStrategy(),
               NoteType.Chain => new ChainJudgeStrategy(),
               NoteType.LongEnd => new LongEndJudgeStrategy(),
            };
            return strategy;
        }
    }
}
