using System;
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
        
        private ReactiveProperty<bool> isHandlingLongNote = new ReactiveProperty<bool>(false);

        private int laneNumber;

        private int pushedFrame = 0;
        private int notPushedFrame = 0;

        public void Initialize()
        {
            this.laneNumber = _laneContext.LaneIndex;
            
            _noteWindow.CurrentNote.Subscribe(note => HandleNoteChange(note)).AddTo(_disposable);

            // 毎フレーム1回だけ入力状態を読む。ロング処理中のみカウントする。
            Observable.EveryUpdate()
                .Where(_ => isHandlingLongNote.Value)
                .Subscribe(_ => CountPushedFrameOnce())
                .AddTo(_disposable);

            isHandlingLongNote
                .DistinctUntilChanged()
                .Subscribe(isHandling =>
                {
                    // ロング始点になった→フレームカウント開始
                    if (isHandling)
                    {
                        pushedFrame = 0;
                        notPushedFrame = 0;
                    }
                    // ロング終点になった→押されたレートをReactivePropertyに書き込んでカウント終了
                    else
                    {
                        int totalFrame = pushedFrame + notPushedFrame;
                        LongPushedRate.Value = totalFrame > 0
                            ? (float)pushedFrame / totalFrame
                            : 0f;
                    }
                })
                .AddTo(_disposable);
        }
        
        public void Dispose()
        {
            _disposable?.Dispose();
            isHandlingLongNote?.Dispose();
            LongPushedRate?.Dispose();
        }
        private void HandleNoteChange(Note note)
        {
            this.laneNumber = note.laneNumber;
            isHandlingLongNote.Value = note switch
            {
                { noteType: NoteType.LongStart, timingDelta: >= 0 } => true,
                { noteType: NoteType.LongEnd, timingDelta: >= 0 } => false,
                _ => isHandlingLongNote.Value
            };
        }

        private void CountPushedFrameOnce()
        {
            if(_playInputLayer.IsButtonPressing[laneNumber].Value == ButtonState.Pushed
               || _playInputLayer.IsButtonPressing[laneNumber].Value == ButtonState.BeginPush
               )
            {
                pushedFrame++;
            }
            else
            {
                notPushedFrame++;
            }
        }
        
    }
}
