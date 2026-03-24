using System.Runtime.CompilerServices.Core.ViewModel;
using Zenject;

namespace OctaNotes.Scripts.Core.ViewModel.Interfaces
{
    public class SceneTransitInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<SceneTransitViewModel>().AsSingle();
        }
    }
}
