using OctaNotes.Scripts.Core.Model;
using OctaNotes.Scripts.SongSelect.Model;
using Zenject;

namespace OctaNotes.Scripts.SongSelect.DI
{
    public class ProjectContextInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GlobalSongDataContext>().AsSingle();
            Container.BindInterfacesAndSelfTo<InputLayer>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<SceneController>().AsSingle();
        }
    }
}
