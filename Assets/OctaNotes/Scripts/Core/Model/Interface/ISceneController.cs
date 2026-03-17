using System;
using Cysharp.Threading.Tasks;
using OctaNotes.Scripts.Core.Model.Structs;

namespace OctaNotes.Scripts.Core.Model.Interface
{
    public interface ISceneController
    {
        public UniTask ChangeScene(Scenes targetScene);
        
        event Func<UniTask> OnSceneLoadBegin;
        event Func<UniTask> OnSceneLoadEnd;
    }
}
