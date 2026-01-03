using UnityEngine;

namespace OctaNotes.Scripts.Play.View
{
    [RequireComponent(typeof(LineRenderer))]
    public class SupportlineMoveView: NoteMoveView
    {
        private LineRenderer _lineRenderer;
        
        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }
        
        public override void setPosZ(double posZ)
        {
            base.setPosZ(posZ);
            
            if (_lineRenderer == null || _lineRenderer.positionCount == 0)
            {
                return;
            }
            
            // 始点の現在のZ位置を取得
            Vector3 firstPos = _lineRenderer.GetPosition(0);
            float deltaZ = (float)posZ - firstPos.z;
            
            // 全ての点を相対的に移動
            for (int i = 0; i < _lineRenderer.positionCount; i++)
            {
                Vector3 pos = _lineRenderer.GetPosition(i);
                pos.z += deltaZ;
                _lineRenderer.SetPosition(i, pos);
            }
        }
    }
}
