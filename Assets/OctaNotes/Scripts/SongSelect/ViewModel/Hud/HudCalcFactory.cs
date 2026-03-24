using OctaNotes.Scripts.SongSelect.ViewModel.Hud.Interface;

namespace OctaNotes.Scripts.SongSelect.ViewModel.Hud
{
    public class HudCalcFactory : IHudCalcFactory
    {
        public IHudParamCalc Create(int hudIdx)
        {
            return hudIdx switch
            {
                0 => new Hud0(),
                1 => new Hud1(),
                2 => new Hud2(),
                3 => new Hud3(),
                4 => new Hud4(),
                5 => new Hud5(),
                6 => new Hud6(),
                7 => new Hud7(),
                _ => null
            };
        }
    }
}
