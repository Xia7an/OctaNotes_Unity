using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Judgment;
using OctaNotes.Scripts.Play.Model.Notes;
using UnityEngine;
using UnityEngine.Assertions;
using Debug = UnityEngine.Debug;

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
        [SerializeField] private GameObject perfectTextPrefab;
        [SerializeField] private GameObject goodTextPrefab;
        [SerializeField] private GameObject badTextPrefab;
        [SerializeField] private GameObject missTextPrefab;
        
        private AudioSource _audioSource;
        
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }
        
        private void OnDestroy()
        {
            cts?.Cancel();
            cts?.Dispose();
        }
        
        public void PresentJudgment(JudgmentEvent judgmentEvent)
        {
            ShowJudgmentText(judgmentEvent.Result, judgmentEvent.NotePosition, judgmentEvent.EffectTime);
            ShowParticle(judgmentEvent.NotePosition, judgmentEvent.EffectTime);
            PlaySound(judgmentEvent.Result);
        }

        private async UniTask ShowJudgmentText(JudgmentResult result, int position, float time)
        {
            // 判定テキストの表示処理
            // TODO: 実際のテキスト表示実装
            Debug.Log($"[JudgmentEffect] {result} at lane {position}, time: {time}");
            Dictionary<int, Vector2> lanePositions = new Dictionary<int, Vector2>
            {
                {0, new Vector2(-400f,250f)},
                {1, new Vector2(-130f, 250f)},
                {2, new Vector2(130f, 250f)},
                {3, new Vector2(400f, 250f)},
                {4, new Vector2(-400f, 780f)},
                {5, new Vector2(-130f, 780f)},
                {6, new Vector2(130f, 780f)},
                {7, new Vector2(400f, 780f)},
            };
            Vector2 spawnPosition = lanePositions.TryGetValue(position, out var lanePosition) ? lanePosition : Vector2.zero;
            GameObject prefab = result switch
            {
                JudgmentResult.Perfect => perfectTextPrefab,
                JudgmentResult.Good => goodTextPrefab,
                JudgmentResult.Bad => badTextPrefab,
                JudgmentResult.Miss => missTextPrefab,
                _ => null
            };
            Assert.IsNotNull(prefab);
            var canvasObj = Instantiate(prefab, spawnPosition, Quaternion.identity);
            var imgRect = canvasObj.GetComponent<JudgementTextComponentRef>().rectTransform;
            imgRect.anchoredPosition = spawnPosition;
            var seq = DOTween.Sequence();
            await seq
                .Append(imgRect.DOScale(1.2f, 0.08f).SetEase(Ease.OutBack))
                .Append(imgRect.DOScale(0f, 0.12f).SetEase(Ease.InBack)).WithCancellation(cts.Token);
            Destroy(canvasObj);
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
            
        }

        public void HideNote(Note note)
        {
            // ノーツを非表示にする処理
            // TODO: 実際のノーツ非表示実装
        }
    }
}
