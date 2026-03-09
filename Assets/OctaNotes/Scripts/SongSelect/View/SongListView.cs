using System.Collections.Generic;
using System.Runtime.CompilerServices.SongSelect.ViewModel.Interface;
using OctaNotes.Scripts.SongSelect.Model.Structs;
using R3;
using UnityEngine;
using Zenject;

namespace System.Runtime.CompilerServices.SongSelect.View
{
    public class SongListView : MonoBehaviour
    {
        [SerializeField] private GameObject songCardPrefab;
        
        private ISongListViewModel _songListViewModel;

        [Inject]
        private void Construct(ISongListViewModel songListViewModel)
        {
            _songListViewModel = songListViewModel;
        }

        private void Start()
        {
            _songListViewModel.SortedSongList.Subscribe(ReConstructSongList).AddTo(this);
        }

        // リストの変更に伴って楽曲ごとのカードリストの再構築を行うメソッド
        // SongCardPrefabをInstantiateし、SongDataの中身をもとに、SongCardViewコンポーネントに値を設定する
        private void ReConstructSongList(List<SongData> sList)
        {
            
        }
    }
}
