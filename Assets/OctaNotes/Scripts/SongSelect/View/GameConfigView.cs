using System.Collections.Generic;
using System.Globalization;
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
    public class GameConfigView : MonoBehaviour
    {
        [SerializeField] private GameObject gameConfigCardPrefab;
        [SerializeField] private GameObject gameConfigListParent;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float tweenDuration = 0.3f;

        private IUIState _uiState;

        private readonly List<GameObject> _cards = new();
        private static readonly Options[] Order = { Options.NoteSpeed, Options.JudgeOffset };
        private static readonly Vector2[] CardPositions =
        {
            new(540f, -50f),
            new(490f, -240f),
            new(240f, -430f),
            new(190f, -670f),
            new(140f, -860f),
        };
        private static readonly Vector2 FarPrevPosition = new(620f, 170f);
        private static readonly Vector2 FarNextPosition = new(100f, -1100f);

        private static readonly Dictionary<Options, string> OptionNames = new()
        {
            { Options.NoteSpeed , "ノーツの速さ"},
            { Options.JudgeOffset , "タイミング調整"}
        };

        [Inject]
        public void Construct(IUIState uiState)
        {
            _uiState = uiState;
        }

        private void Start()
        {
            BuildCards();

            _uiState.State.Select(v => v.cursor.optionIndex)
                .Subscribe(_ =>
                {
                    ApplyAllCards(_uiState.State.Value);
                    PlaceCardsAsync(_uiState.State.Value.cursor.optionIndex, true).Forget();
                })
                .AddTo(this);

            _uiState.State.Select(v => v.noteSpeed)
                .Subscribe(_ => ApplyAllCards(_uiState.State.Value))
                .AddTo(this);

            _uiState.State.Select(v => v.judgeOffsetMs)
                .Subscribe(_ => ApplyAllCards(_uiState.State.Value))
                .AddTo(this);

            _uiState.State.Select(v => v.controlTarget)
                .SubscribeAwait((target, ct) => ToggleCardsAsync(target == Target.GameOptions, ct))
                .AddTo(this);

            ApplyAllCards(_uiState.State.Value);
            PlaceCardsAsync(_uiState.State.Value.cursor.optionIndex, false).Forget();
        }

        private void BuildCards()
        {
            foreach (var card in _cards)
            {
                if (card != null)
                {
                    Destroy(card);
                }
            }

            _cards.Clear();

            for (var i = 0; i < Order.Length; i++)
            {
                var card = Instantiate(gameConfigCardPrefab, gameConfigListParent.transform);
                if (card.TryGetComponent<RectTransform>(out var rt))
                {
                    rt.pivot = new Vector2(0f, 1f);
                    rt.anchorMin = new Vector2(0f, 1f);
                    rt.anchorMax = new Vector2(0f, 1f);
                }

                _cards.Add(card);
            }
        }

        private void ApplyAllCards(UIState state)
        {
            for (var i = 0; i < Order.Length; i++)
            {
                var option = Order[i];
                var card = _cards[i].GetComponent<GameConfigCard>();
                if (card == null)
                {
                    continue;
                }

                var value = option switch
                {
                    Options.NoteSpeed => state.noteSpeed.ToString("0.0", CultureInfo.InvariantCulture),
                    Options.JudgeOffset => (state.judgeOffsetMs/1000f).ToString(CultureInfo.InvariantCulture),
                    _ => string.Empty
                };

                var isSelected = state.selectedOptions == option;
                card.Apply(OptionNames[option], value, isSelected && state.IsOptionValueAtMin, isSelected && state.IsOptionValueAtMax);
            }
        }

        private async UniTask PlaceCardsAsync(int selectedIndex, bool animate)
        {
            if (_cards.Count == 0)
            {
                return;
            }

            var count = _cards.Count;
            var selected = Mathf.Clamp(selectedIndex, 0, count - 1);
            var assignments = new Dictionary<int, Vector2>();
            var scaleMap = new Dictionary<int, float>();

            for (var i = 0; i < count; i++)
            {
                var offset = i - selected;
                var pos = offset switch
                {
                    <= -3 => FarPrevPosition,
                    >= 3 => FarNextPosition,
                    _ => CardPositions[offset + 2]
                };
                assignments[i] = pos;
                scaleMap[i] = offset == 0 ? 1f : 0.75f;
            }

            var tweens = new List<Tween>();

            for (var i = 0; i < count; i++)
            {
                var card = _cards[i];
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
                            new Vector3(targetPos.x, targetPos.y, card.transform.localPosition.z), tweenDuration));
                        tweens.Add(card.transform.DOScale(targetScale, tweenDuration));
                    }
                    else
                    {
                        card.transform.localPosition = new Vector3(targetPos.x, targetPos.y, card.transform.localPosition.z);
                        card.transform.localScale = Vector3.one * targetScale;
                    }
                }
            }

            if (animate && tweens.Count > 0)
            {
                await UniTask.WhenAll(tweens.Select(v => v.ToUniTask()));
            }
        }

        private async UniTask ToggleCardsAsync(bool show, CancellationToken ct)
        {
            await canvasGroup.DOFade(show ? 1f : 0f, 0.1f).ToUniTask(cancellationToken: ct);
        }
    }
}
