using System.Collections.Generic;
using OctaNotes.Scripts.Play.Interface;
using UnityEngine;
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
                var laneNotes = _chartRepositoryImmutable.LaneWiseChartData[i];
                if (currentNoteIndices[i] >= laneNotes.Count) continue;

                var currentNote = laneNotes[currentNoteIndices[i]];
                if (_playInputLayer.IsButtonPressing[i].Value)
                {
                    double currentTime = Time.time;
                    double judgeWindow = 0.1; // 100msのジャッジウィンドウ

                    if (System.Math.Abs(currentTime - currentNote.timing) <= judgeWindow)
                    {
                        // ノーツヒット処理
                        Debug.Log($"a");
                    }
                }
            }
        }
    }
}
