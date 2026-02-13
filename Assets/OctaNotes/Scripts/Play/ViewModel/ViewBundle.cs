using System.Collections.Generic;
using UnityEngine;

namespace OctaNotes.Scripts.Play.ViewModel
{
    public class ViewBundle : MonoBehaviour
    {
        [SerializeField] private List<MonoBehaviour> views = new();

        public IReadOnlyList<MonoBehaviour> Views => views;
    }
}
