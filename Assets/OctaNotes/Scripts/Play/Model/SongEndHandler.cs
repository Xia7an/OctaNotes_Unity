using System;
using Cysharp.Threading.Tasks;
using OctaNotes.Scripts.Core.Model.Interface;
using OctaNotes.Scripts.Core.Model.Structs;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.Model.Struct;
using OctaNotes.Scripts.Play.ViewModel.Interface;
using R3;
using Zenject;

namespace OctaNotes.Scripts.Play.Model
{
    public class SongEndHandler : ISongEndHandler, IInitializable, IDisposable
    {
        private readonly IChartEndDetector chartEndDetector;
        private readonly IGlobalPlayResultContext _globalPlayResultContext;
        private readonly IScoreCalcurator _scoreCalcurator;
        private readonly IComboCalcurator _comboCalcurator;
        private readonly IMusicViewModel  _musicViewModel;
        private readonly ISceneController _sceneController;
        
        
        private CompositeDisposable _disposables = new CompositeDisposable();

        public SongEndHandler(
            IChartEndDetector chartEndDetector,
            IGlobalPlayResultContext globalPlayResultContext,
            IScoreCalcurator scoreCalcurator,
            IComboCalcurator comboCalcurator,
            IMusicViewModel musicViewModel,
            ISceneController sceneController)
        {
            this.chartEndDetector = chartEndDetector;
            _globalPlayResultContext = globalPlayResultContext;
            _scoreCalcurator = scoreCalcurator;
            _comboCalcurator = comboCalcurator;
            _musicViewModel = musicViewModel;
            _sceneController = sceneController;
        }
        
        public void Initialize()
        {
            chartEndDetector.OnSongEnd.Subscribe(SetPlayResult).AddTo(_disposables);
            Observable.FromEvent(
                h => _musicViewModel.OnMusicEnd += h,
                h => _musicViewModel.OnMusicEnd -= h)
                .SubscribeAwait(async (_, ct) =>
                {
                    await UniTask.WaitForSeconds(4, cancellationToken: ct);
                    await _sceneController.ChangeScene(Scenes.Result);
                }).AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }

        private void SetPlayResult(SongEndState state)
        {
            _globalPlayResultContext.ClearMark = state.clearMark;
            _globalPlayResultContext.Score = _scoreCalcurator.Score.Value;
            _globalPlayResultContext.MaxCombo = _comboCalcurator.MaxCombo.Value;
            _globalPlayResultContext.PerfectCount = _scoreCalcurator.perfectCount;
            _globalPlayResultContext.GoodCount = _scoreCalcurator.goodCount;
            _globalPlayResultContext.BadCount = _scoreCalcurator.badCount;
            _globalPlayResultContext.MissCount = _scoreCalcurator.missCount;
        }
    }
}
