using System.Collections.Generic;
using OctaNotes.Scripts.Play.Interface;
using Zenject;

namespace OctaNotes.Scripts.Play.Model
{
    public class JudgeManager: ITickable
    {
        [Inject] private readonly IChartRepositoryImmutable _chartRepositoryImmutable;
        [Inject] private readonly IPlayInputLayer _playInputLayer;
        
        private List<int> currentNoteIndices = new List<int>();

        public JudgeManager()
        {
            for (int i = 0; i < 8; i++)
            {
                currentNoteIndices.Add(0);
            }
        }
        
        public void Tick()
        {
            for (int i = 0; i < 7; i++)
            {
                
            }
        }
    }
}
