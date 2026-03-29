using OctaNotes.Scripts.SongSelect.DI.Hud;
using OctaNotes.Scripts.SongSelect.Model;
using OctaNotes.Scripts.SongSelect.ViewModel.Hud;
using UnityEngine;
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
            Container.BindInterfacesAndSelfTo<SongSelectActionDispatchable>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<InputContextResolver>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<HudCalcFactory>().AsSingle();
        }
    }
}
