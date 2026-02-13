using System;
using System.Linq;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.Model.Struct;
using R3;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.Play.Model
{
    public class JudgeContext : IJudgeContext, IInitializable, IDisposable
    {
        private readonly IPlayInputLayer _inputLayer;
        private readonly INoteWindow _noteWindow;
        private readonly ILongMiddleHandler _longMiddleHandler;
        private readonly IJudgeStrategyFactory _judgeStrategyFactory;
        
        public JudgeContext(
            IPlayInputLayer inputLayer,
            INoteWindow noteWindow,
            ILongMiddleHandler longMiddleHandler,
            IJudgeStrategyFactory judgeStrategyFactory)
        {
            _inputLayer = inputLayer;
            _noteWindow = noteWindow;
            _longMiddleHandler = longMiddleHandler;
            _judgeStrategyFactory = judgeStrategyFactory;
        }

        public ReactiveProperty<JudgeResult> JudgeResult { get; private set; } = new();

        private CompositeDisposable _disposables = new();


        public void Initialize()
        {
            _noteWindow.CurrentNote.Subscribe(Judge).AddTo(_disposables);
        }
        
        public void Dispose()
        {
            _disposables?.Dispose();
            JudgeResult?.Dispose();
        }

        private void Judge(Note note)
        {
            IJudgeStrategy strategy = _judgeStrategyFactory.Create(note.noteType);

            JudgeResult.Value = strategy.JudgeNote(note,
                _inputLayer.IsButtonPressing.Select(state => state.Value).ToList(), 
                _longMiddleHandler.LongPushedRate.Value);
            _printJudgeResult(JudgeResult.Value);
        }

        private void _printJudgeResult(JudgeResult result)
        {
            if(result.laneNumber != 0) return;
            Debug.Log($"[Judge Result] Judge: {result.judge},\n TimingDiff: {result.timingDiff},\n Lane: {result.laneNumber},\n GUID: {result.guid},\n EffectTiming: {result.effectInvokeTiming}");
        }

    }
}
