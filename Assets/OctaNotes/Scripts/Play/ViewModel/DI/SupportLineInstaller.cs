using Zenject;

namespace OctaNotes.Scripts.Play.ViewModel.DI
{
    public class SupportLineInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<SupportLineViewModel>().AsSingle().NonLazy();
        }
    }
}
