using DefaultNamespace;
using OctaNotes.Scripts.SongSelect.Model.Structs;
using OctaNotes.Scripts.SongSelect.ViewModel.Hud.Interface;

namespace OctaNotes.Scripts.SongSelect.ViewModel.Hud
{
    public class Hud0 : IHudParamCalc
    {
        public HudParam Calc(UIState uiState)
        {
            var res = new  HudParam();
            switch (uiState.controlTarget)
            {
                case Target.SongList or Target.GameOptions:
                    res.hudText = "↓下";
                    if (uiState.IsLastSong) res.color = HudColor.Gray;
                    else res.color = HudColor.Green;
                    break;
            }
            return res;
        }
    }
}
