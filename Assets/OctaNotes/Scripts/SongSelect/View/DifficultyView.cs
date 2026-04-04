using OctaNotes.Scripts.Core.Model;
using OctaNotes.Scripts.SongSelect.Model.Interface;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace OctaNotes.Scripts.SongSelect.View
{
    public class DifficultyView : MonoBehaviour
    {
        [SerializeField] private Difficulty difficulty;
        [SerializeField] private Image difficultyImage;
        [SerializeField] private TextMeshProUGUI levelText;
        
        private IUIState _uiState;

        [Inject]
        private void Construct(IUIState uiState)
        {
            _uiState = uiState;
        }

        private void Start()
        {
            _uiState.State.Select(v => v.selectedDifficulty).Subscribe(SetDifficultyColor).AddTo(this);
            _uiState.State.Select(v => v.selectedSongIndex).Subscribe(SetDifficultyLevel).AddTo(this);
        }

        private void SetDifficultyColor(Difficulty selectedDifficulty)
        {
            if (selectedDifficulty == difficulty)
            {
                difficultyImage.color = Color.white;
            }
            else
            {
                difficultyImage.color = new Color(72/255f, 72/255f, 72/255f);
            }
        }
        
        private void SetDifficultyLevel(int songIdx)
        {
            levelText.text = ((int)_uiState.State.Value.songDataList[songIdx].chartDatas[(int)difficulty].level).ToString();
        }
    }
}
