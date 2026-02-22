using System;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.ViewModel.Interface;
using R3;
using Zenject;

namespace OctaNotes.Scripts.Play.ViewModel
{
    public class PlayUIViewModel : IPlayUIViewModel, IInitializable, IDisposable
    {
        private IScoreCalcurator _scoreCalcurator;
        private IComboCalcurator _comboCalcurator;

        public ReactiveProperty<string> ComboText { get; } = new();
        public ReactiveProperty<string> ScoreText { get; } = new();
        
        private CompositeDisposable _disposables = new();

        public PlayUIViewModel(IScoreCalcurator scoreCalcurator, IComboCalcurator comboCalcurator)
        {
            _scoreCalcurator = scoreCalcurator;
            _comboCalcurator = comboCalcurator;
        }

        public void Initialize()
        {
            _scoreCalcurator.Score.Subscribe(v => ScoreText.Value = v.ToString("N0")).AddTo(_disposables);
            _comboCalcurator.Combo.Subscribe(v => ComboText.Value = v.ToString()).AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables?.Dispose();
            ComboText?.Dispose();
            ScoreText?.Dispose();
        }
    }
}
