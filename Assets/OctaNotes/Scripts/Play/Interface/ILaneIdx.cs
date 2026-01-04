namespace OctaNotes.Scripts.Play.Interface
{
    public interface ILaneIdx
    {
        void Advance(int lane);
        int GetHeadIdx(int lane);
    }
}
