using System.Collections.Generic;
using System.IO;
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
        
        private IUIState _uiState;

        private List<GameObject> songCards = new();
        private Vector2 _cardBaseSizeDelta;

        [Inject]
        public void Construct(IUIState uiState)
        {
            _uiState = uiState;
        }

        private void Start()
        {
            _uiState.State.Select(v => v.songDataList).Subscribe(ReConstructSongList).AddTo(this);
            _uiState.State
                .Select(v => v.selectedSongIndex)
                .Subscribe(v => PlaceCards(v, _uiState.State.Value.songDataList))
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
                    rt.pivot = new Vector2(0f, 0.5f);
                    _cardBaseSizeDelta = rt.sizeDelta;
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
            PlaceCards(_uiState.State.Value.selectedSongIndex, songList);
        }

        private void PlaceCards(int selectedSongIdx, List<SongData> songList)
        {
            if (songList == null || songCards.Count == 0)
            {
                return;
            }

            var songCount = songCards.Count;
            var selected = Mod(selectedSongIdx, songCount);

            foreach (var card in songCards)
            {
                if (card != null)
                {
                    card.SetActive(false);
                }
            }

            var placements = new List<(int index, float y, float scale)>
            {
                (selected, 0f, 1f),
                (Mod(selected + 1, songCount), -240f, 0.75f),
                (Mod(selected + 2, songCount), -295f, 0.75f),
                (Mod(selected - 1, songCount), 240f, 0.75f),
                (Mod(selected - 2, songCount), 295f, 0.75f)
            };

            var placedIndexes = new HashSet<int>();
            foreach (var placement in placements)
            {
                if (!placedIndexes.Add(placement.index))
                {
                    continue;
                }

                var card = songCards[placement.index];
                var y = placement.y;
                var x = -55f / 205f * y + 260f;

                if (card.TryGetComponent<RectTransform>(out var rectTransform))
                {
                    rectTransform.anchorMin = new Vector2(0f, 0.5f);
                    rectTransform.anchorMax = new Vector2(0f, 0.5f);
                    rectTransform.anchoredPosition = new Vector2(x, y);
                    rectTransform.sizeDelta = _cardBaseSizeDelta * placement.scale;
                }
                else
                {
                    card.transform.localPosition = new Vector3(x, y, card.transform.localPosition.z);
                }

                card.transform.localScale = Vector3.one * placement.scale;
                card.SetActive(true);
            }
        }

        private int Mod(int value, int modulo)
        {
            var result = value % modulo;
            return result < 0 ? result + modulo : result;
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
