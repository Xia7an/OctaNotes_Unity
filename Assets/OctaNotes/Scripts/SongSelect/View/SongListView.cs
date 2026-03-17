using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        [SerializeField] private Canvas canvas;
        [SerializeField] private float tweenDuration = 0.3f;
        
        private IUIState _uiState;

        private List<GameObject> songCards = new();

        // 仕様の座標 (画面左上基準、コンポーネント左上の角を基準点)
        // anchorMin/Max = (0,1)、pivot = (0,1) として使用する
        // UnityのanchoredPositionはY軸上向きなので符号を反転して格納
        private static readonly Vector2[] CardPositions = new Vector2[]
        {
            new Vector2(560f, -50f),   // 2つ前
            new Vector2(510f, -240f),  // 1つ前
            new Vector2(260f, -430f),  // 選択中
            new Vector2(210f, -670f),  // 1つ次
            new Vector2(160f, -860f),  // 2つ次
        };
        private static readonly Vector2 FarPrevPosition = new Vector2(620f, 170f);   // 3つ前以降 (Y反転)
        private static readonly Vector2 FarNextPosition = new Vector2(100f, -1100f);  // 3つ次以降 (Y反転)

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
                var card = Instantiate(songCardPrefab, canvas.transform);
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

        private void PlaceCards(int selectedSongIdx, List<SongData> songList)
        {
            PlaceCardsAsync(selectedSongIdx, songList, animate: false).Forget();
        }

        private async UniTask PlaceCardsAsync(int selectedSongIdx, List<SongData> songList, bool animate = true)
        {
            if (songList == null || songCards.Count == 0)
            {
                return;
            }

            var songCount = songCards.Count;
            // 循環なし: インデックスを [0, songCount-1] にクランプ
            var selected = Mathf.Clamp(selectedSongIdx, 0, songCount - 1);

            // 各カードのインデックスと配置先を決定する
            // offset: selected基準のオフセット (-: 前, +: 次)
            // CardPositions配列のインデックス: 0=2つ前, 1=1つ前, 2=選択中, 3=1つ次, 4=2つ次
            var assignments = new Dictionary<int, Vector2>();
            var scaleMap = new Dictionary<int, float>();
            var visibilityMap = new Dictionary<int, bool>();

            for (int i = 0; i < songCount; i++)
            {
                int offset = i - selected;

                Vector2 pos;
                float scale;
                bool visible;

                if (offset <= -3)
                {
                    pos = FarPrevPosition;
                    scale = 0.75f;
                    visible = true;
                }
                else if (offset >= 3)
                {
                    pos = FarNextPosition;
                    scale = 0.75f;
                    visible = true;
                }
                else
                {
                    // offset: -2 -> index 0, -1 -> index 1, 0 -> index 2, 1 -> index 3, 2 -> index 4
                    pos = CardPositions[offset + 2];
                    scale = (offset == 0) ? 1f : 0.75f;
                    visible = true;
                }

                assignments[i] = pos;
                scaleMap[i] = scale;
                visibilityMap[i] = visible;
            }

            var tweens = new List<Tween>();

            for (int i = 0; i < songCount; i++)
            {
                var card = songCards[i];
                if (card == null) continue;

                var targetPos = assignments[i];
                var targetScale = scaleMap[i];
                var targetVisible = visibilityMap[i];

                card.SetActive(targetVisible);
                if (!targetVisible) continue;

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
                await UniTask.WhenAll(tweens.Select(t => t.ToUniTask()));
            }
        }

        private Sprite CreateJacketSpriteFromPath(string jacketPath)
        {
            if (string.IsNullOrWhiteSpace(jacketPath) || !File.Exists(jacketPath))
            {
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
