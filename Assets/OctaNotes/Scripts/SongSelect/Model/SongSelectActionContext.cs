using System;
using System.Linq;
using OctaNotes.Scripts.SongSelect.Model.Actions;
using OctaNotes.Scripts.SongSelect.Model.Actions.Interface;
using OctaNotes.Scripts.SongSelect.Model.Interface;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.SongSelect.Model
{
    // InputContextResolverから委譲されたUIActionを受け取り、状態変化を伴わない処理を行うクラス
    public class SongSelectActionContext : ISongSelectActionContext, IInitializable
    {
        private readonly IUIState _uiState;
        private readonly IDispachable _dispachable;
        private readonly ISongRepository _songRepository;
        private readonly IGlobalSongDataContext _globalSongDataContext;

        public SongSelectActionContext(
            IUIState uiState, 
            IGlobalSongDataContext globalSongDataContext,
            IDispachable dispachable,
            ISongRepository songRepository)
        {
            _uiState = uiState;
            _globalSongDataContext = globalSongDataContext;
            _dispachable = dispachable;
            _songRepository = songRepository;
        }
        
        public void Dispatch(UIAction action)
        {
            switch (action)
            {
                case ConfirmSong:
                    _globalSongDataContext.ChartData = 
                        _uiState.State.Value.songDataList[_uiState.State.Value.selectedSongIndex]
                            .chartDatas[(int)_uiState.State.Value.selectedDifficulty];
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
