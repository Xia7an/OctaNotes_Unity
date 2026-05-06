using System;
using R3;
using UnityEngine;

namespace OctaNotes.Scripts.Play.ViewModel.Interface
{
    public interface IMusicViewModel
    {
        ReactiveProperty<AudioClip> AudioClip { get; }

        event Action OnMusicStart;
        event Action OnMusicEnd;
    }
}
