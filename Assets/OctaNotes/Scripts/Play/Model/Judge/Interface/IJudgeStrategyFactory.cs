using OctaNotes.Scripts.Core.Model;

namespace OctaNotes.Scripts.Play.Model.Interface
{
    public interface IJudgeStrategyFactory
    {
        public IJudgeStrategy Create(NoteType noteType);
    }
}
