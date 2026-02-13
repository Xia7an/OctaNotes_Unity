using Zenject;

namespace OctaNotes.Scripts.Play.DI.Lane
{
    public interface ILaneSubContainerFactory
    {
        void BindLane(DiContainer container, int laneIndex);
    }
}
