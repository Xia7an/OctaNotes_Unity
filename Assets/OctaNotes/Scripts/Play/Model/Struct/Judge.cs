namespace OctaNotes.Scripts.Play.Model.Enum
{
    public enum Judge
    {
        NotJudged, // 押されなかったため、判定がなされなかった
        Perfect,
        Good,
        Bad,
        Miss,
        None, // 押したけど判定されなかった
    }
}
