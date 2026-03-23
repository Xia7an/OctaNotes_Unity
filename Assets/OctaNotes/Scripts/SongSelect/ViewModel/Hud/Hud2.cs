using DefaultNamespace;
using OctaNotes.Scripts.SongSelect.Model.Structs;
using OctaNotes.Scripts.SongSelect.ViewModel.Hud.Interface;

namespace OctaNotes.Scripts.SongSelect.ViewModel.Hud
{
    public class Hud2 : IHudParamCalc
    {
        public HudParam Calc(UIState uiState)
        {
            var res = new HudParam();
            res.color = HudColor.Yellow;
            switch (uiState.controlTarget)
            {
                case Target.SongList:
                    res.hudText = "決定";
                    break;
            }
            return res;
        }
    }
}
