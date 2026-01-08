using OctaNotes.Scripts.Core.Model;

namespace OctaNotes.Scripts.Play.Model.Judgment.Processors
{
    /// <summary>
    /// ノーツ種別毎の判定処理を定義するインターフェース
    /// </summary>
    public interface INoteProcessor
    {
        /// <summary>
        /// このProcessorが処理するノーツタイプ
        /// </summary>
        NoteType[] SupportedNoteTypes { get; }

        /// <summary>
        /// ボタン押下時の判定処理
        /// </summary>
        /// <returns>判定結果（判定が発生しない場合はnull）</returns>
        JudgmentOutput? ProcessPress(NoteProcessorContext context);

        /// <summary>
        /// Tickフレーム毎の判定処理（Miss判定など）
        /// </summary>
        /// <returns>判定結果（判定が発生しない場合はnull）</returns>
        JudgmentOutput? ProcessTick(NoteProcessorContext context);
    }
}
