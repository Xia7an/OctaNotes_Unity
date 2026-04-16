using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using OctaNotes.Scripts.Core.Model.Interface;
using OctaNotes.Scripts.Core.Model.Structs;
using OctaNotes.Scripts.SongSelect.Model.Actions;
using OctaNotes.Scripts.SongSelect.Model.Actions.Interface;
using OctaNotes.Scripts.SongSelect.Model.Interface;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.SongSelect.Model
{
    // InputContextResolverから委譲されたUIActionを受け取り、状態変化を伴わない処理を行うクラス
    public class SongSelectActionDispatchable : ISongSelectActionDispatchable, ISongSelectActionEventSource, IInitializable
    {
        private readonly IUIState _uiState;
        private readonly IDispachable _dispachable;
        private readonly ISongRepository _songRepository;
        private readonly IGlobalSongDataContext _globalSongDataContext;
        private readonly ISceneController _sceneController;

        public event Action<UIAction> OnActionDispatched;
        public SongSelectActionDispatchable(
            IUIState uiState, 
            IGlobalSongDataContext globalSongDataContext,
            IDispachable dispachable,
            ISongRepository songRepository,
            ISceneController sceneController)
        {
            _uiState = uiState;
            _globalSongDataContext = globalSongDataContext;
            _dispachable = dispachable;
            _songRepository = songRepository;
            _sceneController = sceneController;
        }
        
        public void Dispatch(UIAction action)
        {
            OnActionDispatched?.Invoke(action);
            switch (action)
            {
                case ConfirmSong:
                    if (_uiState.State.Value.songConfirmed) return;
                    // 今選択されている楽曲の譜面データなどをProjectContext側に渡す
                    _globalSongDataContext.ChartData = 
                        _uiState.State.Value.songDataList[_uiState.State.Value.selectedSongIndex]
                            .chartDatas[(int)_uiState.State.Value.selectedDifficulty];
                    _globalSongDataContext.MusicPath = _uiState.State.Value
                        .songDataList[_uiState.State.Value.selectedSongIndex].musicPath;
                    _sceneController.ChangeScene(Scenes.Play).Forget();
                    break;
            }
        }

        public void Initialize()
        {
            var tmpList = _songRepository.SongDataDict.Values.ToArray();
            _dispachable.Dispatch(new ReloadSongList(tmpList));
        }

    }
}
