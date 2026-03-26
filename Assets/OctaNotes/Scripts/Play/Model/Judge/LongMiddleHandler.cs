using System;
using System.Collections.Generic;
using OctaNotes.Scripts.Core.Model;
using OctaNotes.Scripts.Core.Model.Interface;
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
        
        public LongMiddleHandler(
            IInputLayer inputLayer,
            ILaneContext laneContext)
        {
            this.inputLayer = inputLayer;
            _laneContext = laneContext;
        }
        
        public ReactiveProperty<float> LongPushedRate { get; } = new ReactiveProperty<float>(0);
        
        private CompositeDisposable _disposable = new CompositeDisposable();
        
        private ReactiveProperty<bool> isHandlingLongNote = new ReactiveProperty<bool>(false);
        private Guid _currentLongEndGuid = Guid.Empty;
        private readonly Dictionary<Guid, float> _longEndPushedRates = new();

        private int laneNumber;

        private int pushedFrame = 0;
        private int notPushedFrame = 0;

        public void Initialize()
        {
            this.laneNumber = _laneContext.LaneIndex;

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

                        if (_currentLongEndGuid != Guid.Empty)
                        {
                            _longEndPushedRates[_currentLongEndGuid] = LongPushedRate.Value;
                            _currentLongEndGuid = Guid.Empty;
                        }
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

            var wasHandling = isHandlingLongNote.Value;

            if (wasHandling && note.noteType == NoteType.LongEnd && note.timingDelta >= 0)
            {
                _currentLongEndGuid = note.guid;
                CountPushedFrameOnce();
            }

            var nextHandling = note switch
            {
                { noteType: NoteType.LongStart, timingDelta: >= 0 } => true,
                { noteType: NoteType.LongEnd, timingDelta: >= 0 } => false,
                _ => isHandlingLongNote.Value
            };

            isHandlingLongNote.Value = nextHandling;
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
        
    }
}
