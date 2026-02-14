using System;
using OctaNotes.Scripts.Play.ViewModel;
using UnityEngine;

namespace OctaNotes.Scripts.Play.DI.Lane
{
    [Serializable]
    public class LaneDefinition
    {
        [field: SerializeField] public int LaneId { get; private set; }
        [field: SerializeField] public ViewBundle ViewBundle { get; private set; }
    }
}
