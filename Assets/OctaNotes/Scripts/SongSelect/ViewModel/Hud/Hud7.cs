using DefaultNamespace;
using OctaNotes.Scripts.SongSelect.Model.Structs;
using OctaNotes.Scripts.SongSelect.ViewModel.Hud.Interface;

namespace OctaNotes.Scripts.SongSelect.ViewModel.Hud
{
    public class Hud7 : IHudParamCalc
    {
        public HudParam Calc(UIState uiState)
        {
            var res =  new HudParam();
            switch (uiState.controlTarget)
            {
                case Target.SongList:
                    res.color = HudColor.Cyan;
                    res.hudText = "楽曲\nソート";
                    break;
                default:
                    res.color = HudColor.Black;
                    res.hudText = "";
                    break;
            }
            return res;
        }
    }
}
