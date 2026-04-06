using System.Collections.Generic;
using OctaNotes.Scripts.SongSelect.View;
using UnityEngine;

namespace OctaNotes.Scripts.SongSelect.DI.Hud
{
    public class HudLayout : MonoBehaviour
    {
        [SerializeField] private List<ControllerView> hudViews = new();
        public IReadOnlyList<ControllerView> HUDViews => hudViews;
    }
}
