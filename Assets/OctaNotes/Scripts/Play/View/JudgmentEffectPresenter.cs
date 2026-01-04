using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Judgment;
using OctaNotes.Scripts.Play.Model.Notes;
using UnityEngine;

namespace OctaNotes.Scripts.Play.View
{
    /// <summary>
    /// 判定エフェクトの表示管理
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class JudgmentEffectPresenter : MonoBehaviour, IEffectPresenter
    {
        [SerializeField] private AudioClip perfectSound;
        [SerializeField] private AudioClip goodSound;
        
        private AudioSource _audioSource;
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }
        
        public void PresentJudgment(JudgmentEvent judgmentEvent)
        {
            ShowJudgmentText(judgmentEvent.Result, judgmentEvent.NotePosition, judgmentEvent.EffectTime);
            ShowParticle(judgmentEvent.NotePosition, judgmentEvent.EffectTime);
            PlaySound(judgmentEvent.Result);
        }

        private void ShowJudgmentText(JudgmentResult result, int position, float time)
        {
            // 判定テキストの表示処理
            // TODO: 実際のテキスト表示実装
            Debug.Log($"[JudgmentEffect] {result} at lane {position}, time: {time}");
        }
        
        private void PlaySound(JudgmentResult result)
        {
            AudioClip clip = null;
            switch (result)
            {
                case JudgmentResult.Perfect:
                    clip = perfectSound;
                    break;
                case JudgmentResult.Good:
                    clip = goodSound;
                    break;
                // 他の判定結果に応じたサウンドも追加可能
            }

            if (clip != null)
            {
                _audioSource.PlayOneShot(clip);
            }
        }

        private void ShowParticle(int position, float time)
        {
            // パーティクルエフェクトの表示処理
            // TODO: 実際のパーティクル表示実装
        }

        public void HideNote(Note note)
        {
            // ノーツを非表示にする処理
            // TODO: 実際のノーツ非表示実装
        }
    }
}
