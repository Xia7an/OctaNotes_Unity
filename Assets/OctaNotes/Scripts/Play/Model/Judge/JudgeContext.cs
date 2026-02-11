using System;
using System.Linq;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.Model.Struct;
using R3;
using Zenject;

namespace OctaNotes.Scripts.Play.Model
{
    public class JudgeContext : IJudgeContext, IInitializable, IDisposable
    {
        [Inject] private readonly IPlayInputLayer _inputLayer;
        [Inject] private readonly INoteWindow _noteWindow;
        [Inject] private readonly ILongMiddleHandler _longMiddleHandler;
        [Inject] private readonly IJudgeStrategyFactory _judgeStrategyFactory;
        
        public ReactiveProperty<JudgeResult> JudgeResult { get; private set; }
        
        private CompositeDisposable _disposables;


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
        }

    }
}
