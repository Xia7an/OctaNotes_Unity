using System;
using Cysharp.Threading.Tasks;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.Model.Enum;
using OctaNotes.Scripts.Play.Model.Interface;
using OctaNotes.Scripts.Play.Model.Struct;
using OctaNotes.Scripts.Settings;
using R3;
using UnityEngine;
using Zenject;
using System.Collections.Generic;
using System.Linq;

namespace OctaNotes.Scripts.Play.ViewModel
{
    public class NoteViewModel : INoteViewModel, IInitializable
    {
        private PlaySettingsSO _playSettingsSO;
        private IInGameTimer _inGameTimer;
        private ILaneOutputPort _laneOutputPort;
        private readonly List<(double timing, double hs)> _hsChanges;
        
        public ReactiveProperty<Color> Color { get; } = new();
        public event Action OnJudged;
        public ReactiveProperty<double> PosZ { get; } = new();
        
        private double _initialPosZ = 0;
        private Guid guid = Guid.Empty;
        private CompositeDisposable _disposables = new();
        private bool _isJudged;
        
        public NoteViewModel(
            PlaySettingsSO playSettingsSO,
            IInGameTimer inGameTimer,
            ILaneOutputPort laneOutputPort,
            IChartRepositoryImmutable chartRepository)
        {
            this._playSettingsSO = playSettingsSO;
            this._inGameTimer = inGameTimer;
            this._laneOutputPort = laneOutputPort;
            _hsChanges = chartRepository.HsChangeData
                .OrderBy(x => x.Item1)
                .Select(x => (x.Item1, x.Item2))
                .ToList();
        }
        
        public void SetInitialPosZ(double posZ)
        {
            _initialPosZ = posZ;
        }

        public void SetGuid(Guid guid)
        {
            this.guid = guid;
        }
        

        public void Initialize()
        {
            _inGameTimer.Time.Subscribe(time =>
            {
                var traveledPosition = CalcPositionByHs(time);
                PosZ.Value = -traveledPosition * _playSettingsSO.noteSpeed + _initialPosZ;
            }).AddTo(_disposables);
            
            _laneOutputPort.JudgeResult
                .Subscribe(HandleJudgeResult)
                .AddTo(_disposables);
        }

        private void HandleJudgeResult(JudgeResult result)
        {
            if (_isJudged || guid == Guid.Empty)
            {
                return;
            }

            if (result.guid != guid || result.judge is Judge.NotJudged or Judge.None)
            {
                return;
            }

            ScheduleDeleteNote(result.effectInvokeTiming).Forget();
        }

        private async UniTask ScheduleDeleteNote(float time)
        {
            // エフェクト発動時刻まで待つ
            await UniTask.WaitUntil(() => time <= _inGameTimer.Time.Value);

            if (_isJudged)
            {
                return;
            }

            _isJudged = true;
            OnJudged?.Invoke();
        }

        private double CalcPositionByHs(double time)
        {
            if (_hsChanges.Count == 0)
            {
                return time;
            }

            var pos = 0d;
            var currentTime = 0d;
            var currentHs = 1d;

            for (var i = 0; i < _hsChanges.Count; i++)
            {
                var (changeTime, nextHs) = _hsChanges[i];
                if (changeTime > time)
                {
                    break;
                }

                var dt = changeTime - currentTime;
                if (dt > 0d)
                {
                    pos += currentHs * dt;
                }

                currentTime = changeTime;
                currentHs = nextHs;
            }

            var remain = time - currentTime;
            pos += currentHs * remain;

            return pos;
        }
    }
}
