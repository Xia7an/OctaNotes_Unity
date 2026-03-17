using System;
using Cysharp.Threading.Tasks;

namespace System.Runtime.CompilerServices.Core.ViewModel.Interfaces
{
    public interface ISceneTransitViewModel
    {
        event Func<UniTask> OnSceneLoadBegin;
        event Func<UniTask> OnSceneLoadEnd;
    }
}
