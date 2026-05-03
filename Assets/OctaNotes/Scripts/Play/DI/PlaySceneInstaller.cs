using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.DI.Lane;
using OctaNotes.Scripts.Play.Model;
using System.Linq;
using OctaNotes.Scripts.Core.Model;
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
        Container.BindInterfacesAndSelfTo<InGameTimer>().AsSingle().NonLazy();
        Container.Bind<ILaneSubContainerFactory>().To<LaneSubContainerFactory>().AsSingle();
        Container.BindInterfacesAndSelfTo<ComboCalcurator>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<ScoreCalcurator>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<SongEndDetector>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<SongEndHandler>().AsSingle().NonLazy();
        
        // レーン毎にSubContainerを用意する
        var laneSubContainerFactory = Container.Resolve<ILaneSubContainerFactory>();
        if (laneLayout == null)
        {
            throw new ZenjectException("PlaySceneInstaller.laneLayout is not assigned.");
        }

        // レーン毎に束ねたViewたちのリストを作り、LaneIdでソート
        var laneDefinitions = laneLayout.LaneDefinitions
            .OrderBy(definition => definition.LaneId)
            .ToList();
        // レーンごとにSubContainerを作り、対応するレーンのViewにDI
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
            subContainer.InjectGameObject(laneDefinition.ViewBundle.gameObject);
            
            // 対応するContainerからレーン内のViewに1つずつDI
            foreach (var view in laneDefinition.ViewBundle.Views) 
            {
                if (view == null)
                {
                    continue;
                }

                subContainer.Inject(view);
            }
        }

    }
}
