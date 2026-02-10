using System;
using OctaNotes.Scripts.Title.Interface;
using OctaNotes.Scripts.Title.View;
using UnityEngine;

namespace OctaNotes.Scripts.Title.Model
{
    [Serializable]
    public struct DirectionEvent
    {
        public IDirection Direction;
        public TitleDirectionView View;
        public float At;
    }

}
