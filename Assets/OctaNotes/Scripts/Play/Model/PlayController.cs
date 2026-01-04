using System;
using System.Collections.Generic;
using OctaNotes.Scripts.Play.Interface;
using Zenject;

namespace OctaNotes.Scripts.Play.Model
{
    public class PlayController: ITickable
    {
        [Inject] private readonly IGameTime _gameTime;
        [Inject] private readonly IPlayInputLayer _playInputLayer;
        [Inject] private readonly ILaneIdx _laneHeadIdx;
        [Inject] private readonly IChartRepositoryImmutable _chartRepositoryImmutable;
        
        // 押された瞬間以外に判定されるノーツの判定をここで行う
        public void Tick()
        {
            for (int lane = 0; lane < 8; lane++)
            {
                int headIdx = _laneHeadIdx.GetHeadIdx(lane);
                var notes = _chartRepositoryImmutable.LaneWiseChartData[lane];
                if (headIdx < notes.Count)
                {
                    
                }
            }
            
        }
    }
}
