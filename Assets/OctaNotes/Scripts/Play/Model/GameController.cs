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
        [Inject(Optional = true)] private readonly List<IEffectPresenter> _effectPresenters;
        
        private CompositeDisposable _disposables = new CompositeDisposable();

        public void Initialize()
        {
            // 判定イベントを購読
            _judgmentManager.OnJudgmentEvent
                .Subscribe(HandleJudgmentEvent)
                .AddTo(_disposables);
                
            Debug.Log("GameController initialized");
        }

        public void Tick()
        {
            // 時間更新を発行
            _timeManager?.PublishUpdate();
        }

        private void HandleJudgmentEvent(JudgmentEvent judgmentEvent)
        {
            Debug.Log($"[GameController] Judgment: {judgmentEvent.Result} " +
                      $"Type: {judgmentEvent.JudgmentType} " +
                      $"Lane: {judgmentEvent.NotePosition}");
            
            // 全てのEffectPresenterに通知
            if (_effectPresenters != null)
            {
                foreach (var presenter in _effectPresenters)
                {
                    presenter.PresentJudgment(judgmentEvent);
                }
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
