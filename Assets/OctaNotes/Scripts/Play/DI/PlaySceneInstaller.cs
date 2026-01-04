using OctaNotes.Scripts.Play.Model;
using UnityEngine;
using Zenject;

public class PlaySceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Debug.Log("[PlaySceneInstaller] InstallBindings called");

        Container.BindInterfacesAndSelfTo<ChartRepository>().AsSingle();
        Container.BindInterfacesAndSelfTo<ChartParser>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<InputController>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<TimeManager>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<JudgmentManager>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<GameController>().AsSingle().NonLazy();
    }
}
