using System.Collections.Generic;
using UnityEngine;

namespace OctaNotes.Scripts.Play.DI.Lane
{
    public class LaneLayout : MonoBehaviour
    {
        [SerializeField] private List<LaneDefinition> laneDefinitions = new();
        public IReadOnlyList<LaneDefinition> LaneDefinitions => laneDefinitions;
    }
}
