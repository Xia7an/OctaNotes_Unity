using System;
using System.Collections.Generic;
using System.Linq;
using OctaNotes.Scripts.Core.Model;
using OctaNotes.Scripts.Core.Model.Interface;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.Model.Struct;
using OctaNotes.Scripts.Settings;
using R3;
using Zenject;

namespace OctaNotes.Scripts.Play.Model
{
    public class JudgeContext : IJudgeContext, IInitializable, IDisposable
    {
        private readonly IInputLayer _inputLayer;
        private readonly INoteWindow _noteWindow;
        private readonly ILongMiddleHandler _longMiddleHandler;
        private readonly IJudgeStrategyFactory _judgeStrategyFactory;
        private readonly IInGameTimer _inGameTimer;
        private readonly IChartRepositoryImmutable _chartRepository;
        private readonly ILaneContext _laneContext;
        private readonly PlaySettingsSO _playSettings;

        private Guid _lastJudgedNoteGuid = Guid.Empty;
        private readonly HashSet<Guid> _judgedNoteGuids = new();
        private int _nextMissCheckIndex;
        
        public JudgeContext(
            IInputLayer inputLayer,
            INoteWindow noteWindow,
            ILongMiddleHandler longMiddleHandler,
            IJudgeStrategyFactory judgeStrategyFactory,
            IInGameTimer inGameTimer,
            IChartRepositoryImmutable chartRepository,
            ILaneContext laneContext,
            PlaySettingsSO playSettings)
        {
            _inputLayer = inputLayer;
            _noteWindow = noteWindow;
            _longMiddleHandler = longMiddleHandler;
            _judgeStrategyFactory = judgeStrategyFactory;
            _inGameTimer = inGameTimer;
            _chartRepository = chartRepository;
            _laneContext = laneContext;
            _playSettings = playSettings;
        }

        public ReactiveProperty<JudgeResult> JudgeResult { get; private set; } = new();

        private CompositeDisposable _disposables = new();


        public void Initialize()
        {
            _noteWindow.CurrentNote.Subscribe(Judge).AddTo(_disposables);
            _inGameTimer.Time.Subscribe(CheckTimeoutMiss).AddTo(_disposables);
        }
        
        public void Dispose()
        {
            _disposables?.Dispose();
            JudgeResult?.Dispose();
        }

        private void Judge(Note note)
        {
            if (note.guid == Guid.Empty) return;
            if (note.guid == _lastJudgedNoteGuid) return;
            
            IJudgeStrategy strategy = _judgeStrategyFactory.Create(note.noteType, note.isEx);
            var result = strategy.JudgeNote(note,
                _inputLayer.IsButtonPressing.Select(state => state.Value).ToList(), 
                _longMiddleHandler.LongPushedRate.Value);

            if (result.judge is Enum.Judge.NotJudged or Enum.Judge.None) return;
            if (_judgedNoteGuids.Contains(note.guid)) return;

            JudgeResult.Value = result;
            _lastJudgedNoteGuid = note.guid;
            _judgedNoteGuids.Add(note.guid);
            _noteWindow.NotifyJudged(note.guid);
            _printJudgeResult(JudgeResult.Value);
        }

        private void CheckTimeoutMiss(float currentTime)
        {
            var laneIndex = _laneContext.LaneIndex;
            if (laneIndex < 0 || laneIndex >= _chartRepository.LaneWiseChartData.Count)
            {
                return;
            }

            var laneNotes = _chartRepository.LaneWiseChartData[laneIndex];
            var badThresholdSec = _playSettings.badRangeMs / 1000f;

            while (_nextMissCheckIndex < laneNotes.Count)
            {
                var note = laneNotes[_nextMissCheckIndex];
                var missTiming = (float)note.timing + badThresholdSec;
                if (currentTime < missTiming)
                {
                    break;
                }

                if (!_judgedNoteGuids.Contains(note.guid) && IsTimeoutMissTarget(note.noteType))
                {
                    var missResult = new JudgeResult
                    {
                        judge = Enum.Judge.Miss,
                        timingDiff = TimingDiff.Late,
                        laneNumber = laneIndex,
                        guid = note.guid,
                        isEx = note.isEx,
                        effectInvokeTiming = missTiming
                    };
                    JudgeResult.Value = missResult;
                    _lastJudgedNoteGuid = note.guid;
                    _judgedNoteGuids.Add(note.guid);
                    _noteWindow.NotifyJudged(note.guid);
                    _printJudgeResult(missResult);
                }

                _nextMissCheckIndex++;
            }
        }

        private static bool IsTimeoutMissTarget(NoteType noteType)
        {
            return noteType is NoteType.Tap or NoteType.Chain or NoteType.LongStart;
        }

        private void _printJudgeResult(JudgeResult result)
        {
            // if(result.judge == Enum.Judge.NotJudged) return;
            // Debug.Log($"[Judge Result] Judge: {result.judge},\n" +
            //           $"TimingDiff: {result.timingDiff},\n " +
            //           $"Lane: {result.laneNumber},\n " +
            //           $"GUID: {result.guid},\n " +
            //           $"EffectTiming: {result.effectInvokeTiming}, \n " +
            //           $"LongPushedRate: {_longMiddleHandler.LongPushedRate.Value}");
        }

    }
}
