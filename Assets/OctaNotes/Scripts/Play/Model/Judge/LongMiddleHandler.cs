using System;
using System.Collections.Generic;
using OctaNotes.Scripts.Core.Model;
using OctaNotes.Scripts.Core.Model.Interface;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.Model.Struct;
using R3;
using Zenject;

namespace OctaNotes.Scripts.Play.Model
{
    public class LongMiddleHandler : ILongMiddleHandler, IInitializable, IDisposable
    {
        private readonly IInputLayer inputLayer;
        private readonly ILaneContext _laneContext;
        private readonly IChartRepositoryImmutable _chartRepository;
        
        public LongMiddleHandler(
            IInputLayer inputLayer,
            ILaneContext laneContext,
            IChartRepositoryImmutable chartRepository)
        {
            this.inputLayer = inputLayer;
            _laneContext = laneContext;
            _chartRepository = chartRepository;
        }
        
        public ReactiveProperty<float> LongPushedRate { get; } = new ReactiveProperty<float>(0);
        
        private readonly CompositeDisposable _disposable = new CompositeDisposable();

        private bool _isHandlingLongNote;
        private readonly Dictionary<Guid, float> _longEndPushedRates = new();
        private Guid _activeLongGuid = Guid.Empty;
        private readonly Dictionary<Guid, Guid> _longStartToEndGuid = new();

        private int laneNumber;

        private int pushedFrame;
        private int notPushedFrame;

        public void Initialize()
        {
            this.laneNumber = _laneContext.LaneIndex;
            BuildLongGuidMap();

            Observable.EveryUpdate()
                .Where(_ => _isHandlingLongNote)
                .Subscribe(_ => CountPushedFrameOnce())
                .AddTo(_disposable);
        }
        
        public void Dispose()
        {
            _disposable?.Dispose();
            LongPushedRate?.Dispose();
        }

        private void HandleNoteChange(Note note)
        {
            this.laneNumber = note.laneNumber;

            if (_isHandlingLongNote && note.noteType == NoteType.LongEnd && note.timingDelta >= 0)
            {
                if (_activeLongGuid != note.guid)
                {
                    return;
                }

                CountPushedFrameOnce();
                FinalizeCurrentLong(note.guid);
                _isHandlingLongNote = false;
                _activeLongGuid = Guid.Empty;
            }
        }

        private void CountPushedFrameOnce()
        {
            if(inputLayer.IsButtonPressing[laneNumber].Value == ButtonState.Pushed
               || inputLayer.IsButtonPressing[laneNumber].Value == ButtonState.BeginPush
               )
            {
                pushedFrame++;
            }
            else
            {
                notPushedFrame++;
            }
        }

        private void FinalizeCurrentLong(Guid longEndGuid)
        {
            int totalFrame = pushedFrame + notPushedFrame;
            var pushedRate = totalFrame > 0
                ? (float)pushedFrame / totalFrame
                : 0f;

            LongPushedRate.Value = pushedRate;
            if (longEndGuid != Guid.Empty)
            {
                _longEndPushedRates[longEndGuid] = pushedRate;
            }
        }

        public bool TryGetLongEndPushedRate(Guid longEndGuid, out float pushedRate)
        {
            if (longEndGuid == Guid.Empty)
            {
                pushedRate = 0f;
                return false;
            }

            return _longEndPushedRates.TryGetValue(longEndGuid, out pushedRate);
        }

        public void SyncWithCurrentNote(Note note)
        {
            HandleNoteChange(note);
        }

        public void NotifyLongStartJudged(Note note)
        {
            if (note.noteType != NoteType.LongStart)
            {
                return;
            }

            _isHandlingLongNote = true;
            _activeLongGuid = _longStartToEndGuid.TryGetValue(note.guid, out var longEndGuid)
                ? longEndGuid
                : Guid.Empty;
            pushedFrame = 0;
            notPushedFrame = 0;
        }

        private void BuildLongGuidMap()
        {
            _longStartToEndGuid.Clear();

            var laneIndex = _laneContext.LaneIndex;
            var laneWise = _chartRepository.LaneWiseChartData;
            if (laneIndex < 0 || laneIndex >= laneWise.Count)
            {
                return;
            }

            var laneNotes = laneWise[laneIndex];
            var waitingLongStarts = new Queue<Guid>();
            for (var i = 0; i < laneNotes.Count; i++)
            {
                var note = laneNotes[i];
                if (note.noteType == NoteType.LongStart && note.guid != Guid.Empty)
                {
                    waitingLongStarts.Enqueue(note.guid);
                    continue;
                }

                if (note.noteType == NoteType.LongEnd && note.guid != Guid.Empty && waitingLongStarts.Count > 0)
                {
                    var startGuid = waitingLongStarts.Dequeue();
                    _longStartToEndGuid[startGuid] = note.guid;
                }
            }
        }
        
    }
}
