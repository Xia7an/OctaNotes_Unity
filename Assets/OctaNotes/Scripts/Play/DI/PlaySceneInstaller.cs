using UnityEngine;
using Zenject;

public class PlaySceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<OctaNotes.Scripts.Play.Model.ChartRepository>().AsSingle();
        Container.BindInterfacesAndSelfTo<OctaNotes.Scripts.Play.Model.ChartParser>().AsSingle().NonLazy();
    }
}
