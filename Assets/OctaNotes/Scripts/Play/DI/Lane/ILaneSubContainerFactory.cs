using OctaNotes.Scripts.Play.View;
using Zenject;

namespace OctaNotes.Scripts.Play.DI.Lane
{
    public interface ILaneSubContainerFactory
    {
        DiContainer CreateLaneSubContainer(int laneIndex);
        void BindLane(DiContainer container, int laneIndex, ILaneView[] laneViews);
        DiContainer GetLaneSubContainer(int laneIndex);
        T ResolveFromLane<T>(int laneIndex);
        T ResolveFromLaneId<T>(int laneIndex, object identifier);
    }
}
