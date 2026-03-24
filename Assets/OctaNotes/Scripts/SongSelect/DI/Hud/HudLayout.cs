using System.Collections.Generic;
using OctaNotes.Scripts.SongSelect.View;
using UnityEngine;

namespace OctaNotes.Scripts.SongSelect.DI.Hud
{
    public class HudLayout : MonoBehaviour
    {
        [SerializeField] private List<HudView> hudViews = new();
        public IReadOnlyList<HudView> HUDViews => hudViews;
    }
}
