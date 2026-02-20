using OctaNotes.Scripts.Play.Interface;
using R3;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.Play.View
{
    [RequireComponent(typeof(LineRenderer))]
    public class SupportlineMoveView: MonoBehaviour
    {
        ISupportLineViewModel _lineViewModel;
        
        private LineRenderer _lineRenderer;

        [Inject]
        public void Construct(ISupportLineViewModel lineViewModel)
        {
            _lineViewModel = lineViewModel;
        }
        
        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        private void Start()
        {
            _lineViewModel.PosZ.Subscribe(SetPosZ).AddTo(this);
            
            Observable.FromEvent(h => _lineViewModel.OnJudged += h, 
                h => _lineViewModel.OnJudged -= h)
                .Subscribe(_ =>InvokeJudgedEffect()).AddTo(this);
        }
        
        
        private void SetPosZ(double posZ)
        {
            
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

        private void InvokeJudgedEffect()
        {
            Destroy(gameObject);
        }
    }
}
