using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.DI.Lane;
using OctaNotes.Scripts.Play.Model;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class PlaySceneInstaller : MonoInstaller
{
    [SerializeField] private LaneLayout laneLayout;

    public override void InstallBindings()
    {
        Debug.Log("[PlaySceneInstaller] InstallBindings called");

        Container.BindInterfacesAndSelfTo<ChartRepository>().AsSingle();
        Container.BindInterfacesAndSelfTo<ChartParser>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<PlayInputLayer>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<InGameTimer>().AsSingle().NonLazy();
        Container.Bind<ILaneSubContainerFactory>().To<LaneSubContainerFactory>().AsSingle();
        var laneSubContainerFactory = Container.Resolve<ILaneSubContainerFactory>();
        if (laneLayout == null)
        {
            throw new ZenjectException("PlaySceneInstaller.laneLayout is not assigned.");
        }

        var laneDefinitions = laneLayout.LaneDefinitions
            .OrderBy(definition => definition.LaneId)
            .ToList();
        var laneContainers = new List<DiContainer>(laneDefinitions.Count);
        var laneInputPorts = new List<ILaneInputPort>(laneDefinitions.Count);

        for (int expectedLaneId = 0; expectedLaneId < laneDefinitions.Count; expectedLaneId++)
        {
            var laneDefinition = laneDefinitions[expectedLaneId];
            if (laneDefinition.LaneId != expectedLaneId)
            {
                throw new ZenjectException($"LaneDefinition ids must be contiguous from 0. Missing lane {expectedLaneId}.");
            }

            if (laneDefinition.ViewBundle == null)
            {
                throw new ZenjectException($"ViewBundle for lane {laneDefinition.LaneId} is not assigned.");
            }

            var subContainer = laneSubContainerFactory.CreateLaneSubContainer(laneDefinition.LaneId);
            laneContainers.Add(subContainer);
            subContainer.InjectGameObject(laneDefinition.ViewBundle.gameObject);

            foreach (var view in laneDefinition.ViewBundle.Views)
            {
                if (view == null)
                {
                    continue;
                }

                subContainer.Inject(view);
            }

            laneInputPorts.Add(subContainer.Resolve<ILaneInputPort>());
        }

        Container.Bind<List<ILaneInputPort>>().FromInstance(laneInputPorts).AsSingle();
        Container.Bind<List<ILaneOutputPort>>()
            .FromMethod(_ => laneContainers.Select(subContainer => subContainer.Resolve<ILaneOutputPort>()).ToList())
            .AsSingle();
        Container.BindInterfacesAndSelfTo<LaneInputManager>().AsSingle().NonLazy();
    }
}
