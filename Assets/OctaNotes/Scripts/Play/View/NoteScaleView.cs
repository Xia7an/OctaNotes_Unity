using System;
using UnityEngine;

namespace OctaNotes.Scripts.Play.View
{
    public class NoteScaleView : MonoBehaviour
    {
        private const float JudgelineZ = 0;

        private void Update()
        {
            var z = this.transform.position.z;
            this.transform.localScale = new Vector3(0.1f, 1, Scale(z -  JudgelineZ));
        }

        private float Scale(float distance)
        {
            // if (distance <= 2) return 0.08f;
            return distance * 0.005f + 0.08f;
        }
    }
}
