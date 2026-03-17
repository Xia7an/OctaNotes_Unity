using OctaNotes.Scripts.Title.Model;
using Zenject;

namespace OctaNotes.Scripts.Title.DI
{
    public class TitleInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<InputContextHandler>().AsSingle().NonLazy();
        }
    }
}
