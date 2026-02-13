using System;
using System.Collections.Generic;
using System.Threading;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.Model.Struct;
using R3;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.Play.Model
{
    public class NoteWindow: INoteWindow, IDisposable, IInitializable
    {
        private readonly IInGameTimer inGameTimer;
        private readonly IChartRepositoryImmutable _chartRepository;
        private readonly ILaneContext _laneContext;

        private readonly CompositeDisposable _disposables = new();
        private int _laneIndex;
        private int _lastClosestIndex = -1;

        public ReactiveProperty<Note> CurrentNote { get; private set; } = new();
        
        public NoteWindow(
            IInGameTimer inGameTimer,
            IChartRepositoryImmutable chartRepository,
            ILaneContext laneContext)
        {
            this.inGameTimer = inGameTimer;
            _chartRepository = chartRepository;
            _laneContext = laneContext;
        }

        public void Initialize()
        {
            this._laneIndex = _laneContext.LaneIndex;
            inGameTimer.Time.Subscribe(v => GetCurrentNote(v, _chartRepository.LaneWiseChartData)).AddTo(_disposables);
        }
        
        public void Dispose()
        {
            CurrentNote?.Dispose();
            _disposables.Dispose();
        }
        

        private void GetCurrentNote(float timer, List<List<NoteTiming>> chartData)
        {
            // 入力やレーンの妥当性チェック
            // Find the nearest note within the target lane only.
            if (chartData == null || _laneIndex < 0 || _laneIndex >= chartData.Count)
            {
                return;
            }

            List<NoteTiming> lane = chartData[_laneIndex];
            if (lane == null || lane.Count == 0)
            {
                return;
            }

            int closestIndex = -1;
            double closestDiff = double.MaxValue;

            // 前回インデックスを使ったO(1)近傍判定
            // 最も近くのノーツが、前回実行時に選択されたノーツ、もしくは前回実行時に選択されたノーツの次のノーツであると仮定して、
            // それが最も近くであることが確定できる場合には、そのノーツを選択する。
            if (_lastClosestIndex >= 0 && _lastClosestIndex < lane.Count)
            {
                int prevIndex = _lastClosestIndex;
                int nextIndex = prevIndex + 1;
                int beforeIndex = prevIndex - 1;

                double t1 = Math.Abs(lane[prevIndex].timing - timer);
                double t2 = beforeIndex >= 0 ? Math.Abs(lane[beforeIndex].timing - timer) : double.MaxValue;
                double t3 = nextIndex < lane.Count ? Math.Abs(lane[nextIndex].timing - timer) : double.MaxValue;

                if (t1 <= t2 && t1 <= t3)
                {
                    closestIndex = prevIndex;
                    closestDiff = t1;
                }
                else if (nextIndex < lane.Count)
                {
                    int nextNextIndex = nextIndex + 1;
                    double t4 = nextNextIndex < lane.Count
                        ? Math.Abs(lane[nextNextIndex].timing - timer)
                        : double.MaxValue;

                    if (t3 <= t1 && t3 <= t4)
                    {
                        closestIndex = nextIndex;
                        closestDiff = t3;
                    }
                }
            }

            // O(1)で確定できない場合は二分探索へフォールバック
            if (closestIndex < 0)
            {
                int left = 0;
                int right = lane.Count;
                while (left < right)
                {
                    int mid = left + (right - left) / 2;
                    if (lane[mid].timing < timer)
                    {
                        left = mid + 1;
                    }
                    else
                    {
                        right = mid;
                    }
                }

                // Check candidate at lower-bound and the previous one.
                for (int i = left; i >= left - 1; i--)
                {
                    if (i < 0 || i >= lane.Count)
                    {
                        continue;
                    }

                    double diff = Math.Abs(lane[i].timing - timer);
                    if (diff < closestDiff)
                    {
                        closestDiff = diff;
                        closestIndex = i;
                    }
                }
            }

            // 最終的な最寄りノーツが見つからない場合は終了
            if (closestIndex < 0)
            {
                return;
            }

            _lastClosestIndex = closestIndex;
            NoteTiming closestTiming = lane[closestIndex];

            // 最寄りノーツ情報を使ってNoteを生成し通知
            Note note = new Note()
            {
                guid = closestTiming.guid,
                justTiming = (float)closestTiming.timing,
                laneNumber = _laneIndex,
                noteType = closestTiming.noteType,
                timingDelta = (float)(timer - closestTiming.timing)
            };
            
            CurrentNote.Value = note;
            // _printCurrentNote(CurrentNote.Value);
        }

        private void _printCurrentNote(Note note)
        {
            if(note.laneNumber != 0) return;
            Debug.Log($"[Current Note] Type: {note.noteType},\n GUID: {note.guid},\n Lane: {note.laneNumber}");
        }

    }
}
