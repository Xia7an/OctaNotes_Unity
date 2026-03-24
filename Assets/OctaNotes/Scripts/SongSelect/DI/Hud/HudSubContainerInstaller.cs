using OctaNotes.Scripts.SongSelect.ViewModel;
using OctaNotes.Scripts.SongSelect.ViewModel.Hud;
using OctaNotes.Scripts.SongSelect.ViewModel.Interface;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.SongSelect.DI.Hud
{
    public class HudSubContainerInstaller :  MonoInstaller
    {
        [SerializeField] private int hudIdx;
        
        public override void InstallBindings()
        {
            // ViewModelにHudのIdxを渡す
            Container.BindInterfacesAndSelfTo<HudViewModel>().AsSingle().WithArguments(hudIdx);
        }
    }
}
