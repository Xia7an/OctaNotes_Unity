using DefaultNamespace;
using OctaNotes.Scripts.SongSelect.Model.Structs;
using OctaNotes.Scripts.SongSelect.ViewModel.Hud.Interface;

namespace OctaNotes.Scripts.SongSelect.ViewModel.Hud
{
    public class Hud4 : IHudParamCalc
    {
        public HudParam Calc(UIState uiState)
        {
            var res =  new HudParam();
            switch (uiState.controlTarget)
            {
                case Target.SongList:
                    res.color = HudColor.Orange;
                    res.hudText = "レベル\n-";
                    break;
                default:
                    res.color = HudColor.Orange;
                    res.hudText = "-";
                    break;
            }
            return res;
        }
    }
}
