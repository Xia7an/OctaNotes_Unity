using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.Model.JudgeStrategies;
using Zenject;

namespace OctaNotes.Scripts.Play.DI.Lane
{
    public class LaneSubContainerInstaller : Installer<int, LaneSubContainerInstaller>
    {
        private readonly int _laneIndex;

        public LaneSubContainerInstaller(int laneIndex)
        {
            _laneIndex = laneIndex;
        }

        public override void InstallBindings()
        {
            Container.Bind<ILaneContext>().To<LaneContextAdapter>().AsSingle().WithArguments(_laneIndex);

            Container.BindInterfacesAndSelfTo<LaneScopedInputLayer>().AsSingle();

            Container.BindInterfacesAndSelfTo<NoteWindow>().AsSingle();
            Container.BindInterfacesAndSelfTo<LongMiddleHandler>().AsSingle();
            Container.BindInterfacesAndSelfTo<JudgeStrategyFactory>().AsSingle();
            Container.BindInterfacesAndSelfTo<JudgeContext>().AsSingle();

            Container.Bind<ILaneInputPort>().To<LaneInputPort>().AsSingle();
            Container.Bind<ILaneOutputPort>().To<LaneOutputPort>().AsSingle();
            Container.BindInterfacesAndSelfTo<LaneViewModel>().AsSingle();
        }
    }
}
