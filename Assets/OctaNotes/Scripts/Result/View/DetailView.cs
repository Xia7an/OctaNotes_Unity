using OctaNotes.Scripts.Play.Model.Enum;
using OctaNotes.Scripts.Play.Model.Interface;
using TMPro;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.Result.View
{
    public class DetailView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI countText;
        [SerializeField] private Judge judge;
        
        private IGlobalPlayResultContext _globalPlayResultContext;

        [Inject]
        private void Construct(IGlobalPlayResultContext globalPlayResultContext)
        {
            _globalPlayResultContext = globalPlayResultContext;
        }

        private void Start()
        {
            var count = judge switch
            {
                Judge.Perfect => _globalPlayResultContext.PerfectCount,
                Judge.Good => _globalPlayResultContext.GoodCount,
                Judge.Bad => _globalPlayResultContext.BadCount,
                Judge.Miss => _globalPlayResultContext.MissCount
            };
            SetCountText(count);
        }

        private void SetCountText(int count)
        {
            countText.text = count.ToString();
        }
    }
}
