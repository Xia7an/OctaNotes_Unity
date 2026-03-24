using System;
using System.Collections.Generic;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.Model.Struct;
using OctaNotes.Scripts.Settings;
using R3;
using Zenject;

namespace OctaNotes.Scripts.Play.Model
{
    public class NoteWindow: INoteWindow, IDisposable, IInitializable
    {
        private readonly IInGameTimer inGameTimer;
        private readonly IChartRepositoryImmutable _chartRepository;
        private readonly ILaneContext _laneContext;
        private readonly PlaySettingsSO _playSettings;

        private readonly CompositeDisposable _disposables = new();
        private int _laneIndex;
        private int _nextCandidateIndex;
        private readonly Dictionary<Guid, int> _noteIndexByGuid = new();

        public ReactiveProperty<Note> CurrentNote { get; private set; } = new();
        
        public NoteWindow(
            IInGameTimer inGameTimer,
            IChartRepositoryImmutable chartRepository,
            ILaneContext laneContext,
            PlaySettingsSO playSettings)
        {
            this.inGameTimer = inGameTimer;
            _chartRepository = chartRepository;
            _laneContext = laneContext;
            _playSettings = playSettings;
        }

        public void Initialize()
        {
            this._laneIndex = _laneContext.LaneIndex;
            BuildGuidIndexMap();
            inGameTimer.Time.Subscribe(v => GetCurrentNote(v, _chartRepository.LaneWiseChartData)).AddTo(_disposables);
        }
        
        public void Dispose()
        {
            CurrentNote?.Dispose();
            _disposables.Dispose();
        }

        public void NotifyJudged(Guid noteGuid)
        {
            if (noteGuid == Guid.Empty)
            {
                return;
            }

            if (_noteIndexByGuid.TryGetValue(noteGuid, out var judgedIndex))
            {
                _nextCandidateIndex = Math.Max(_nextCandidateIndex, judgedIndex + 1);
            }
        }

        private void BuildGuidIndexMap()
        {
            _noteIndexByGuid.Clear();

            var laneNotes = _chartRepository.LaneWiseChartData;
            if (_laneIndex < 0 || _laneIndex >= laneNotes.Count)
            {
                return;
            }

            var lane = laneNotes[_laneIndex];
            for (var i = 0; i < lane.Count; i++)
            {
                var guid = lane[i].guid;
                if (guid == Guid.Empty || _noteIndexByGuid.ContainsKey(guid))
                {
                    continue;
                }

                _noteIndexByGuid[guid] = i;
            }
        }
        

        private void GetCurrentNote(float timer, List<List<NoteTiming>> chartData)
        {
            if (chartData == null || _laneIndex < 0 || _laneIndex >= chartData.Count)
            {
                return;
            }

            List<NoteTiming> lane = chartData[_laneIndex];
            if (lane == null || lane.Count == 0)
            {
                return;
            }

            float badThresholdSec = _playSettings.badRangeMs / 1000f;
            float timeoutBorder = timer - badThresholdSec;

            while (_nextCandidateIndex < lane.Count && lane[_nextCandidateIndex].timing < timeoutBorder)
            {
                _nextCandidateIndex++;
            }

            if (_nextCandidateIndex >= lane.Count)
            {
                return;
            }

            NoteTiming closestTiming = lane[_nextCandidateIndex];

            Note note = new Note()
            {
                guid = closestTiming.guid,
                justTiming = (float)closestTiming.timing,
                laneNumber = _laneIndex,
                noteType = closestTiming.noteType,
                timingDelta = (float)(timer - closestTiming.timing),
                isEx = closestTiming.isEx,
            };
            
            CurrentNote.Value = note;
        }

    }
}
