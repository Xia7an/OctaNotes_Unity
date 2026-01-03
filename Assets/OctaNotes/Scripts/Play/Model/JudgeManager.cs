using System;
using System.Collections.Generic;
using System.Threading;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Settings;
using UnityEngine;
using Zenject;
using R3;

namespace OctaNotes.Scripts.Play.Model
{
    public class JudgeManager: IInitializable, ITickable, IDisposable
    {
        [Inject] private readonly IChartRepositoryImmutable _chartRepositoryImmutable;
        [Inject] private readonly IPlayInputLayer _playInputLayer;
        [Inject] private readonly PlaySettingsSO _playSettingsSO;
        
        private List<int> currentNoteIndices = new List<int>();
        
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public JudgeManager()
        {
            Debug.Log("JudgeManager initialized");
            for (int i = 0; i < 8; i++)
            {
                currentNoteIndices.Add(0);
            }
        }
        
        public void Initialize()
        {
            // タップノーツの判定処理を登録
            _playInputLayer.IsButtonPressing[0].Subscribe(isPressing =>
            {
                if (isPressing)
                {
                    JudgeTap(0, Time.time);
                }
            });
            
            _playInputLayer.IsButtonPressing[1].Subscribe(isPressing =>
            {
                if (isPressing)
                {
                    JudgeTap(1, Time.time);
                }
            });
            
            _playInputLayer.IsButtonPressing[2].Subscribe(isPressing =>
            {
                if (isPressing)
                {
                    JudgeTap(2, Time.time);
                }
            });
            
            _playInputLayer.IsButtonPressing[3].Subscribe(isPressing =>
            {
                if (isPressing)
                {
                    JudgeTap(3, Time.time);
                }
            });
            
            _playInputLayer.IsButtonPressing[4].Subscribe(isPressing =>
            {
                if (isPressing)
                {
                    JudgeTap(4, Time.time);
                }
            });
            
            _playInputLayer.IsButtonPressing[5].Subscribe(isPressing =>
            {
                if (isPressing)
                {
                    JudgeTap(5, Time.time);
                }
            });
            
            _playInputLayer.IsButtonPressing[6].Subscribe(isPressing =>
            {
                if (isPressing)
                {
                    JudgeTap(6, Time.time);
                }
            });
            
            _playInputLayer.IsButtonPressing[7].Subscribe(isPressing =>
            {
                if (isPressing)
                {
                    JudgeTap(7, Time.time);
                }
            });
            
            // レーン0のノートのタイミング差が0.15未満になったら発火するイベント
            Observable.EveryUpdate()
                .Where(_ => 
                {
                    if (currentNoteIndices[0] >= _chartRepositoryImmutable.LaneWiseChartData[0].Count)
                        return false;
                    var noteTiming = _chartRepositoryImmutable.LaneWiseChartData[0][currentNoteIndices[0]].timing;
                    var delta = Math.Abs(noteTiming + _playSettingsSO.songStartDelay - Time.time);
                    return delta < 0.15;
                })
                .Subscribe(_ =>
                {
                    var noteTiming = _chartRepositoryImmutable.LaneWiseChartData[0][currentNoteIndices[0]].timing;
                    var delta = Math.Abs(noteTiming + _playSettingsSO.songStartDelay - Time.time);
                    Debug.Log($"Lane 0 Note {currentNoteIndices[0]} is within 0.15s! Delta: {delta}");
                });
        }

        public void Tick()
        {
            // 遅すぎでMissになったノーツを進める
            for (int laneIndex = 0; laneIndex < 8; laneIndex++)
            {
                var noteList = _chartRepositoryImmutable.LaneWiseChartData[laneIndex];
                var noteIndex = currentNoteIndices[laneIndex];
                if (noteIndex >= noteList.Count) continue;
                var noteTiming = noteList[noteIndex].timing;
                var delta = Time.time - (noteTiming + _playSettingsSO.songStartDelay);
                if (delta > _playSettingsSO.maxMissDelayMs/1000.0)
                {
                    Debug.Log($"Lane {laneIndex} Note {noteIndex} Judged: MISS");
                    currentNoteIndices[laneIndex]++;
                }
            }
        }
        
        private void JudgeTap(int laneIndex, double timing)
        {
            var noteList = _chartRepositoryImmutable.LaneWiseChartData[laneIndex];
            var noteIndex = currentNoteIndices[laneIndex];
            if (noteIndex >= noteList.Count) return;
            var noteTiming = noteList[noteIndex].timing;
            var delta = Math.Abs(noteTiming + _playSettingsSO.songStartDelay  - timing);
            var isLate = noteTiming + _playSettingsSO.songStartDelay  - timing > 0;
            if (delta <= _playSettingsSO.perfectRangeMs/1000.0)
            {
                Debug.Log($"Lane {laneIndex} Note {noteIndex} Judged: PERFECT");
                currentNoteIndices[laneIndex]++;
            }
            else if (delta <= _playSettingsSO.goodRangeMs/1000.0)
            {
                Debug.Log($"Lane {laneIndex} Note {noteIndex} Judged: GOOD");
                currentNoteIndices[laneIndex]++;
            }
            else if (delta <= _playSettingsSO.badRangeMs/1000.0)
            {
                Debug.Log($"Lane {laneIndex} Note {noteIndex} Judged: BAD");
                currentNoteIndices[laneIndex]++;
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
