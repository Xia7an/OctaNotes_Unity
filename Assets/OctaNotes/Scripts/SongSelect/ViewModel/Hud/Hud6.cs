using OctaNotes.Scripts.SongSelect.Model.Structs;
using OctaNotes.Scripts.SongSelect.ViewModel.Hud.Interface;

namespace OctaNotes.Scripts.SongSelect.ViewModel.Hud
{
    public class Hud6 : IHudParamCalc
    {
        public HudParam Calc(UIState uiState)
        {
            return new HudParam()
            {
                color = HudColor.Black,
                hudText = "",
            };
        }
    }
}
