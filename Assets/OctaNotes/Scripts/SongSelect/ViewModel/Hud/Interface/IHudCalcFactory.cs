namespace OctaNotes.Scripts.SongSelect.ViewModel.Hud.Interface
{
    public interface IHudCalcFactory
    {
        IHudParamCalc Create(int hudIdx);
    }
}
