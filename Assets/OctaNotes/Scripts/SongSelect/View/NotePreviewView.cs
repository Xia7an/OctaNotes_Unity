using OctaNotes.Scripts.Settings;
using OctaNotes.Scripts.SongSelect.Model.Interface;
using R3;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.SongSelect.View
{
    public class NotePreviewView : MonoBehaviour
    {
        private IUIState _uiState;

        private double noteSpeed;

        private double _initialPosZ = 8f;

        private double PosZ;

        [Inject]
        private void Construct(IUIState uiState)
        {
            _uiState = uiState;
        }

        private void Start()
        {
            PosZ = _initialPosZ;
            _uiState.State
                .Select(v => v.noteSpeed)
                .DistinctUntilChanged()
                .Subscribe(v => noteSpeed = v).AddTo(this);
        }

        private void Update()
        {
            PosZ -= noteSpeed * Time.deltaTime;
            if (PosZ < -2) PosZ = _initialPosZ;
            this.transform.localPosition = new Vector3(
                -0.5f,
                0.01f,
                (float)PosZ);
        }
    }
}
