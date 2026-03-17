using OctaNotes.Scripts.SongSelect.Model;
using Zenject;

namespace OctaNotes.Scripts.SongSelect.DI
{
    public class SongSelectInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<SongRepository>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<Reducer>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<UIStateStore>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<SongSelectActionContext>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<InputContextResolver>().AsSingle().NonLazy();
        }
    }
}
