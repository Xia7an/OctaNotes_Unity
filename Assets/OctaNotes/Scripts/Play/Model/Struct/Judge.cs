namespace OctaNotes.Scripts.Play.Model.Enum
{
    public enum Judge
    {
        Perfect,
        Good,
        Bad,
        Miss,
        None, // 押したけど判定されなかった
        NotJudged, // 押されなかったため、判定がなされなかった
    }
}
