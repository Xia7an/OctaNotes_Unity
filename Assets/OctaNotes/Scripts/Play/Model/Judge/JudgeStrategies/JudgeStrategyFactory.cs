using OctaNotes.Scripts.Core.Model;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Settings;

namespace OctaNotes.Scripts.Play.Model.JudgeStrategies
{
    public class JudgeStrategyFactory : IJudgeStrategyFactory
    {
        private readonly PlaySettingsSO _playSettings;

        public JudgeStrategyFactory(PlaySettingsSO playSettings)
        {
            _playSettings = playSettings;
        }
        
        
        public IJudgeStrategy Create(NoteType noteType, bool isEx)
        { 
            IJudgeStrategy strategy = noteType switch
            {
               (NoteType.Tap or NoteType.LongStart) when !isEx => new TapJudgeStrategy(_playSettings), // Exでない場合は通常の判定
               (NoteType.Tap or NoteType.LongStart) when (isEx) => new ExTapJudgeStrategy(_playSettings),
               NoteType.Chain => new ChainJudgeStrategy(_playSettings),
               NoteType.LongEnd => new LongEndJudgeStrategy(),
            };
            return strategy;
        }
    }
}
