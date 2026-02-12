using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using OctaNotes.Scripts.Core.Model;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.Model.Struct;
using R3;
using Zenject;

namespace OctaNotes.Scripts.Play.Model
{
    public class LongMiddleHandler : ILongMiddleHandler, IInitializable, IDisposable
    {
        private readonly IPlayInputLayer _playInputLayer;
        private readonly INoteWindow _noteWindow;
        private readonly ILaneContext _laneContext;
        
        public LongMiddleHandler(
            IPlayInputLayer playInputLayer,
            INoteWindow noteWindow,
            ILaneContext laneContext)
        {
            _playInputLayer = playInputLayer;
            _noteWindow = noteWindow;
            _laneContext = laneContext;
        }
        
        public ReactiveProperty<float> LongPushedRate { get; } = new ReactiveProperty<float>(0);
        
        private CompositeDisposable _disposable = new CompositeDisposable();
        private CancellationTokenSource _longNoteCts = new CancellationTokenSource();
        
        private ReactiveProperty<bool> isHandlingLongNote = new ReactiveProperty<bool>(false);

        private int laneNumber;
        

        public void Initialize()
        {
            this.laneNumber = _laneContext.LaneIndex;
            
            _noteWindow.CurrentNote.Subscribe(note => HandleNoteChange(note)).AddTo(_disposable);
            isHandlingLongNote
                .DistinctUntilChanged()
                .Subscribe(isHandling =>
                {
                    if (isHandling)
                    {
                        _longNoteCts?.Cancel();
                        _longNoteCts?.Dispose();
                        _longNoteCts = new CancellationTokenSource();

                        UniTask.Void(async () =>
                        {
                            var rate = await CountPushedFrame(this.laneNumber, _longNoteCts.Token);
                            LongPushedRate.Value = rate;
                        });
                    }
                    else
                    {
                        _longNoteCts?.Cancel();
                    }
                })
                .AddTo(_disposable);
        }
        
        public void Dispose()
        {
            _longNoteCts?.Cancel();
            _longNoteCts?.Dispose();
            _disposable?.Dispose();
            LongPushedRate?.Dispose();
        }
        private void HandleNoteChange(Note note)
        {
            this.laneNumber = note.laneNumber;
            if(note.noteType == NoteType.LongStart) isHandlingLongNote.Value = true;
            else if (note.noteType == NoteType.LongEnd) isHandlingLongNote.Value = false;
        }

        private async UniTask<float> CountPushedFrame(
            int laneNumber,
            CancellationToken token
            )
        {
            int pushedFrame = 0;
            int notPushedFrame = 0;
            while (isHandlingLongNote.Value)
            {
                if(_playInputLayer.IsButtonPressing[laneNumber].Value == ButtonState.Pushed
                   || _playInputLayer.IsButtonPressing[laneNumber].Value == ButtonState.BeginPush
                   ) pushedFrame++;
                else notPushedFrame++;
                await UniTask.Yield(token);
            }

            return (float)pushedFrame / (float)(notPushedFrame + pushedFrame);
        }
        
    }
}
