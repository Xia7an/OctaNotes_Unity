using System;
using OctaNotes.Scripts.SongSelect.Model.Interface;
using OctaNotes.Scripts.SongSelect.Model.Structs;
using OctaNotes.Scripts.SongSelect.ViewModel.Hud.Interface;
using OctaNotes.Scripts.SongSelect.ViewModel.Interface;
using R3;
using Zenject;

namespace OctaNotes.Scripts.SongSelect.ViewModel
{
    public class HudViewModel : IHudViewModel, IInitializable, IDisposable
    {
        private readonly IHudCalcFactory _hudCalcFactory;
        private readonly IUIState _uiState;
        
        public ReactiveProperty<HudParam> Param { get; } =  new ReactiveProperty<HudParam>();

        private int _hudIdx;

        private IHudParamCalc _calculator;
        private CompositeDisposable _disposables = new CompositeDisposable();

        public HudViewModel(int hudIdx, IHudCalcFactory hudCalcFactory,  IUIState uiState)
        {
            _hudIdx = hudIdx;
            _hudCalcFactory = hudCalcFactory;
            _uiState = uiState;
        }

        public void Initialize()
        {
            _calculator = _hudCalcFactory.Create(_hudIdx);
            _uiState.State.Subscribe(v => Param.Value = _calculator.Calc(v)).AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables?.Dispose();
            Param?.Dispose();
        }
    }
}
