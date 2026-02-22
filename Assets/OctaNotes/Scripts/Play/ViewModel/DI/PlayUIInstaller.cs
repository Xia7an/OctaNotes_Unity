using Zenject;

namespace OctaNotes.Scripts.Play.ViewModel.DI
{
    public class PlayUIInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PlayUIViewModel>().AsSingle().NonLazy();
        }
    }
}
