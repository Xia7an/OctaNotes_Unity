using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Enum;
using OctaNotes.Scripts.Play.Model.Struct;
using R3;
using UnityEngine;
using UnityEngine.VFX;
using Zenject;

namespace OctaNotes.Scripts.Play.View
{
    public class JudgeEffectView : MonoBehaviour
    {
        [SerializeField] private VisualEffect vfx;
        
        private ILaneViewModel _laneViewModel;
        
        private enum JudgeNum
        {
            Perfect = 0,
            Good = 1,
            Bad = 2
        }

        [Inject]
        private void Construct([InjectOptional] ILaneViewModel laneViewModel)
        {
            if (_laneViewModel == null && laneViewModel != null)
            {
                _laneViewModel = laneViewModel;
            }
        }


        private void Start()
        {
            _laneViewModel.CurrentJudge.Where(IsTappedJudge).Subscribe(ShowEffect).AddTo(this);
        }

        private void ShowEffect(Judge judge)
        {
            var judgeNum = judge switch
            {
                Judge.Perfect => JudgeNum.Perfect,
                Judge.Good => JudgeNum.Good,
                Judge.Bad => JudgeNum.Bad
            };
            vfx.SetInt("Judge",  (int)judgeNum);
            vfx.SendEvent("OnPlay");
        }

        private bool IsTappedJudge(Judge judge)
        {
            return judge is Judge.Perfect or Judge.Good  or Judge.Bad;
        }
        
    }
    
}
