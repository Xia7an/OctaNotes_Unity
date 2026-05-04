using OctaNotes.Scripts.Core.Model;
using OctaNotes.Scripts.Play.Model.Interface;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace OctaNotes.Scripts.Result.View
{
    public class DifficultyView : MonoBehaviour
    {
        [SerializeField] private Sprite DualBG;
        [SerializeField] private Sprite QuadBG;
        [SerializeField] private Sprite OctaBG;
        
        [SerializeField] private Image BGImage;
        [SerializeField] private TextMeshProUGUI levelText;
        
        private IGlobalPlayResultContext _globalPlayResultContext;

        [Inject]
        private void Construct(IGlobalPlayResultContext globalPlayResultContext)
        {
            _globalPlayResultContext =  globalPlayResultContext;
        }

        private void Start()
        {
            SetDifficultyAndLevel(_globalPlayResultContext.Difficulty, 
                (int)_globalPlayResultContext.SongData.chartDatas[(int)_globalPlayResultContext.Difficulty].level);
        }

        private void SetDifficultyAndLevel(Difficulty difficulty,  int level)
        {
            BGImage.sprite = difficulty switch
            {
                Difficulty.Dual => DualBG,
                Difficulty.Quad => QuadBG,
                Difficulty.Octa => OctaBG,
            };
            levelText.text = level.ToString();
        }
    }
}
