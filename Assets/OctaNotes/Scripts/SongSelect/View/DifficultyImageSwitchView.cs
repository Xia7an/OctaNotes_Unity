using OctaNotes.Scripts.Core.Model;
using OctaNotes.Scripts.SongSelect.Model.Interface;
using OctaNotes.Scripts.SongSelect.Model.Structs;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace OctaNotes.Scripts.SongSelect.View
{
    public class DifficultyImageSwitchView : MonoBehaviour
    {
        [SerializeField] private Difficulty difficulty;
        [SerializeField] private Image targetImage;
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite specialSprite;
        
        private IUIState _uiState;

        [Inject]
        private void Construct(IUIState uiState)
        {
            _uiState = uiState;
        }

        private void Start()
        {
            _uiState.State.Subscribe(v => SetImage(IsSpecial(v))).AddTo(this);
        }

        private void SetImage(bool isSpecial)
        {
            targetImage.sprite = isSpecial ? specialSprite : normalSprite;
        }

        private bool IsSpecial(UIState state)
        {
            return state.songDataList[state.selectedSongIndex].chartDatas[(int)difficulty].level >= 10;
        }
    }
}
