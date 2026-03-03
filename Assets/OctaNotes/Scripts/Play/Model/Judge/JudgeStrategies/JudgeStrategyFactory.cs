using System.Collections.Generic;
using OctaNotes.Scripts.Core.Model;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Settings;

namespace OctaNotes.Scripts.Play.Model.JudgeStrategies
{
    public class JudgeStrategyFactory : IJudgeStrategyFactory
    {
        private readonly PlaySettingsSO _playSettings;
        private readonly Dictionary<(NoteType noteType, bool isEx), IJudgeStrategy> _strategyCache = new();
        

        public JudgeStrategyFactory(PlaySettingsSO playSettings)
        {
            _playSettings = playSettings;
        }
        
        
        public IJudgeStrategy Create(NoteType noteType, bool isEx)
        {
            var key = (noteType, isEx);
            
            // 生成済みならキャッシュを返す
            if (_strategyCache.TryGetValue(key, out var cachedStrategy))
            {
                return cachedStrategy;
            }
            
            // 未生成なら必要に応じて生成する
            IJudgeStrategy strategy = noteType switch
            {
               (NoteType.Tap or NoteType.LongStart) when !isEx 
                    => new TapJudgeStrategy(_playSettings), // Exでない場合は通常の判定
               (NoteType.Tap or NoteType.LongStart) when (isEx) 
                    => new ExTapJudgeStrategy(_playSettings),
                NoteType.Chain 
                    => new ChainJudgeStrategy(_playSettings),
                NoteType.LongEnd => new LongEndJudgeStrategy(),
            };
            _strategyCache[key] = strategy;
            return strategy;
        }
    }
}
