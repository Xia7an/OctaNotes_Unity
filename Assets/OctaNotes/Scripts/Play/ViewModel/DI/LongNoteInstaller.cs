using Zenject;

namespace OctaNotes.Scripts.Play.ViewModel.DI
{
    public class LongNoteInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<LongNoteViewModel>().AsSingle().NonLazy();
        }
    }
}
