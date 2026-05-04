using OctaNotes.Scripts.Result.Model;
using Zenject;

namespace System.Runtime.CompilerServices.Result.DI
{
    public class ResultInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<InputContextHandler>().AsSingle().NonLazy();
        }
    }
}
