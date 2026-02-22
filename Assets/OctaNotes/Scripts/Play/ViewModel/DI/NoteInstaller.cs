using Zenject;

namespace OctaNotes.Scripts.Play.ViewModel.DI
{
    public class NoteInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<NoteViewModel>().AsSingle().NonLazy();
        }
    }
}
