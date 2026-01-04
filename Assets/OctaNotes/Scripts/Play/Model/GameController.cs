using System;
using System.Collections.Generic;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Judgment;
using R3;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.Play.Model
{
    /// <summary>
    /// ゲームコントローラー
    /// 判定システム全体を統合する
    /// </summary>
    public class GameController : IInitializable, ITickable, IDisposable
    {
        [Inject] private readonly JudgmentManager _judgmentManager;
        [Inject] private readonly IInputController _inputController;
        [Inject] private readonly TimeManager _timeManager;

        private readonly List<IEffectPresenter> _effectPresenters;
        private CompositeDisposable _disposables = new CompositeDisposable();

        [Inject]
        public GameController([InjectOptional] List<IEffectPresenter> effectPresenters)
        {
            _effectPresenters = effectPresenters ?? new List<IEffectPresenter>();
            Debug.Log($"[GameController] Constructor called, {_effectPresenters.Count} presenters injected");
        }

        public void Initialize()
        {
            Debug.Log($"[GameController] Initialize called, subscribing to JudgmentManager events");

            // 判定イベントを購読
            _judgmentManager.OnJudgmentEvent
                .Subscribe(HandleJudgmentEvent)
                .AddTo(_disposables);

            Debug.Log($"[GameController] Initialized with {_effectPresenters.Count} effect presenters");
        }

        public void Tick()
        {
            // 時間更新を発行
            _timeManager?.PublishUpdate();
        }

        private void HandleJudgmentEvent(JudgmentEvent judgmentEvent)
        {
            Debug.Log($"[GameController] HandleJudgmentEvent: {judgmentEvent.Result} " +
                      $"Type: {judgmentEvent.JudgmentType} " +
                      $"Lane: {judgmentEvent.NotePosition}");

            // 全てのEffectPresenterに通知
            foreach (var presenter in _effectPresenters)
            {
                Debug.Log($"[GameController] Calling presenter: {presenter.GetType().Name}");
                presenter.PresentJudgment(judgmentEvent);
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
