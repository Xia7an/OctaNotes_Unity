using System;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.Model.Struct;
using R3;
using Zenject;

namespace OctaNotes.Scripts.Play.Model
{
    public class SongEndHandler : ISongEndHandler, IInitializable, IDisposable
    {
        private readonly ISongEndDetector _songEndDetector;
        private readonly IGlobalPlayResultContext _globalPlayResultContext;
        private readonly IScoreCalcurator _scoreCalcurator;
        private readonly IComboCalcurator _comboCalcurator;
        
        
        private CompositeDisposable _disposables;

        public SongEndHandler(
            ISongEndDetector songEndDetector,
            IGlobalPlayResultContext globalPlayResultContext,
            IScoreCalcurator scoreCalcurator,
            IComboCalcurator comboCalcurator)
        {
            _songEndDetector = songEndDetector;
            _globalPlayResultContext = globalPlayResultContext;
            _scoreCalcurator = scoreCalcurator;
            _comboCalcurator = comboCalcurator;
        }
        
        public void Initialize()
        {
            _songEndDetector.OnSongEnd.Subscribe(SetPlayResult).AddTo(_disposables);
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
        }
    }
}
