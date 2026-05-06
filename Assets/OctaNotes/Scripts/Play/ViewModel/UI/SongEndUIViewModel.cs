using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using OctaNotes.Scripts.Core.Model.Interface;
using OctaNotes.Scripts.Core.Model.Structs;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Struct;
using OctaNotes.Scripts.Play.ViewModel.Interface;
using R3;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.Play.ViewModel
{
    public class SongEndUIViewModel : ISongEndUIViewModel, IInitializable, IDisposable
    {
        private readonly IChartEndDetector chartEndDetector;
        
        public ReactiveProperty<ClearMark> ShowClearMark { get; } = new ReactiveProperty<ClearMark>();
        
        private CompositeDisposable _disposables = new CompositeDisposable();

        public SongEndUIViewModel(IChartEndDetector chartEndDetector)
        {
            this.chartEndDetector = chartEndDetector;
        }

        public void Initialize()
        {
            chartEndDetector.OnSongEnd.Skip(1)
                .SubscribeAwait((v, ct) => HandleSongEnd(v, ct))
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private async UniTask HandleSongEnd(SongEndState songEndState, CancellationToken ct)
        {
            if (songEndState.clearMark is ClearMark.Clear or ClearMark.Failed)
            {
                await UniTask.WaitForSeconds(2,cancellationToken: ct);
            }

            ShowClearMark.OnNext(songEndState.clearMark);
        }
    }
}
