using OctaNotes.Scripts.SongSelect.Model.Structs;

namespace OctaNotes.Scripts.SongSelect.ViewModel.Hud.Interface
{
    public interface IHudParamCalc
    {
        HudParam Calc(UIState uiState);
    }
}
