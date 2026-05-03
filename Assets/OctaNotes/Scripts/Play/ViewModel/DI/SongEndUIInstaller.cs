using Zenject;

namespace OctaNotes.Scripts.Play.ViewModel.DI
{
    public class SongEndUIInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<SongEndUIViewModel>().AsSingle();
        }
    }
}
