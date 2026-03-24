using OctaNotes.Scripts.Play.Model.Struct;
using R3;
using System;

namespace OctaNotes.Scripts.Play.Model.Interface
{
    public interface INoteWindow
    {
        ReactiveProperty<Note> CurrentNote { get; }
        void NotifyJudged(Guid noteGuid);
    }
}
