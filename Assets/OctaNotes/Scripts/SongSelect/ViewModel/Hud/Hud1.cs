using OctaNotes.Scripts.SongSelect.Model.Structs;
using OctaNotes.Scripts.SongSelect.ViewModel.Hud.Interface;

namespace OctaNotes.Scripts.SongSelect.ViewModel.Hud
{
    public class Hud1 : IHudParamCalc
    {
        public HudParam Calc(UIState uiState)
        {
            var res = new  HudParam();
            switch (uiState.controlTarget)
            {
                case Target.SongList or Target.GameOptions:
                    res.hudText = "↑上";
                    if (uiState.IsFirstSong) res.color = HudColor.Gray;
                    else res.color = HudColor.Green;
                    break;
            }
            return res;
        }
    }
}
