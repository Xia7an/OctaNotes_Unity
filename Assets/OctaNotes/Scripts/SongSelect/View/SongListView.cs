using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using OctaNotes.Scripts.SongSelect.Model.Interface;
using OctaNotes.Scripts.SongSelect.Model.Structs;
using R3;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.SongSelect.View
{
    public class SongListView : MonoBehaviour
    {
        [SerializeField] private GameObject songCardPrefab;
        [SerializeField] private GameObject songListParent;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float tweenDuration = 0.3f;
        
        private IUIState _uiState;

        private List<GameObject> songCards = new();

        private static readonly Vector2[] CardPositions =
        {
            new(560f, -50f),
            new(510f, -240f),
            new(260f, -430f),
            new(210f, -670f),
            new(160f, -860f),
        };
        private static readonly Vector2 FarPrevPosition = new(620f, 170f);
        private static readonly Vector2 FarNextPosition = new(100f, -1100f);

        [Inject]
        public void Construct(IUIState uiState)
        {
            _uiState = uiState;
        }

        private void Start()
        {
            _uiState.State.Select(v => v.songDataList).DistinctUntilChanged().Subscribe(ReConstructSongList).AddTo(this);
            _uiState.State
                .Select(v => v.selectedSongIndex)
                .Subscribe(v => PlaceCardsAsync(v, _uiState.State.Value.songDataList).Forget())
                .AddTo(this);
            
            _uiState.State
                .Select(v => v.controlTarget)
                .SubscribeAwait((target, ct) => ToggleAllCardsAsync(target == Target.SongList, ct))
                .AddTo(this);
        }

        // リストの変更に伴って楽曲ごとのカードリストの再構築を行うメソッド
        // SongCardPrefabをInstantiateし、SongDataの中身をもとに、SongCardViewコンポーネントに値を設定する
        private void ReConstructSongList(List<SongData> songList)
        {
            Debug.Log($"Re-constructing {songList.Count} songs");
            foreach (var card in songCards)
            {
                if (card != null)
                {
                    Destroy(card);
                }
            }

            var tmp = new List<GameObject>();
            if (songList == null)
            {
                songCards = tmp;
                return;
            }

            foreach (var songData in songList)
            {
                var card = Instantiate(songCardPrefab, songListParent.transform);
                if (card.TryGetComponent<RectTransform>(out var rt))
                {
                    rt.pivot = new Vector2(0f, 1f);
                    rt.anchorMin = new Vector2(0f, 1f);
                    rt.anchorMax = new Vector2(0f, 1f);
                }
                var jacketSprite = CreateJacketSpriteFromPath(songData.jacketPath);
                card.GetComponent<SongCardView>().Apply(
                    songData.songName,
                    songData.composerName,
                    songData.chartDatas[0].level,
                    songData.chartDatas[1].level,
                    songData.chartDatas[2].level,
                    jacketSprite);
                tmp.Add(card);
            }
            songCards = tmp;
            PlaceCardsAsync(_uiState.State.Value.selectedSongIndex, songList, animate: false).Forget();
        }
        
        private async UniTask PlaceCardsAsync(int selectedSongIdx, List<SongData> songList, bool animate = true)
        {
            if (songList == null || songCards.Count == 0)
            {
                return;
            }

            var songCount = songCards.Count;
            var selected = Mathf.Clamp(selectedSongIdx, 0, songCount - 1);

            var assignments = new Dictionary<int, Vector2>();
            var scaleMap = new Dictionary<int, float>();

            for (var i = 0; i < songCount; i++)
            {
                var offset = i - selected;
                var pos = offset switch
                {
                    <= -3 => FarPrevPosition,
                    >= 3 => FarNextPosition,
                    _ => CardPositions[offset + 2]
                };
                var scale = offset == 0 ? 1f : 0.75f;

                assignments[i] = pos;
                scaleMap[i] = scale;
            }

            var tweens = new List<Tween>();

            for (var i = 0; i < songCount; i++)
            {
                var card = songCards[i];
                if (card == null) continue;

                var targetPos = assignments[i];
                var targetScale = scaleMap[i];

                card.SetActive(true);

                if (card.TryGetComponent<RectTransform>(out var rt))
                {
                    rt.anchorMin = new Vector2(0f, 1f);
                    rt.anchorMax = new Vector2(0f, 1f);
                    rt.pivot = new Vector2(0f, 1f);

                    if (animate)
                    {
                        tweens.Add(rt.DOAnchorPos(targetPos, tweenDuration));
                        tweens.Add(card.transform.DOScale(targetScale, tweenDuration));
                    }
                    else
                    {
                        rt.anchoredPosition = targetPos;
                        card.transform.localScale = Vector3.one * targetScale;
                    }
                }
                else
                {
                    if (animate)
                    {
                        tweens.Add(card.transform.DOLocalMove(
                            new Vector3(targetPos.x, targetPos.y, card.transform.localPosition.z),
                            tweenDuration));
                        tweens.Add(card.transform.DOScale(targetScale, tweenDuration));
                    }
                    else
                    {
                        card.transform.localPosition = new Vector3(targetPos.x, targetPos.y,
                            card.transform.localPosition.z);
                        card.transform.localScale = Vector3.one * targetScale;
                    }
                }
            }

            if (animate && tweens.Count > 0)
            {
                await UniTask.WhenAll(tweens.Select(v => v.ToUniTask()));
            }
        }

        private async UniTask ToggleAllCardsAsync(bool show, CancellationToken ct)
        {
            await canvasGroup.DOFade(show ? 1f : 0, 0.1f).ToUniTask(cancellationToken: ct);
        }

        private Sprite CreateJacketSpriteFromPath(string jacketPath)
        {
            if (string.IsNullOrWhiteSpace(jacketPath) || !File.Exists(jacketPath))
            {
                Debug.LogWarning($"{jacketPath} not found.");
                return null;
            }

            var bytes = File.ReadAllBytes(jacketPath);
            var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!texture.LoadImage(bytes))
            {
                Destroy(texture);
                return null;
            }

            return Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f));
        }
    }
}
