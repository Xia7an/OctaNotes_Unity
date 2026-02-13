using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.DI.Lane;
using OctaNotes.Scripts.Play.Model;
using UnityEngine;
using Zenject;

public class PlaySceneInstaller : MonoInstaller
{
    private const int LaneCount = 8;

    public override void InstallBindings()
    {
        Debug.Log("[PlaySceneInstaller] InstallBindings called");

        Container.BindInterfacesAndSelfTo<ChartRepository>().AsSingle();
        Container.BindInterfacesAndSelfTo<ChartParser>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<PlayInputLayer>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<GameTimer>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<LaneSubContainerFactory>().AsSingle();

        ILaneSubContainerFactory laneSubContainerFactory = Container.Resolve<ILaneSubContainerFactory>();
        for (int lane = 0; lane < LaneCount; lane++)
        {
            laneSubContainerFactory.BindLane(Container, lane);
        }

        Container.BindInterfacesAndSelfTo<LaneInputManager>().AsSingle().NonLazy();
    }
}
