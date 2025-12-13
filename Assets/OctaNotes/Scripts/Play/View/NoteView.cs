using UnityEngine;

namespace OctaNotes.Scripts.Play.View
{
    public class NoteView : MonoBehaviour
    {
        [SerializeField] private MeshRenderer noteRenderer;
        private void SetColor(Color color)
        {
            noteRenderer.material.color = color;
        }
    }
}
