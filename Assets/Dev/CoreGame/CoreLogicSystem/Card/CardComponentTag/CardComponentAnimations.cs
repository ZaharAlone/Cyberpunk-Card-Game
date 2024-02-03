using System;
using UnityEngine;
using DG.Tweening;

namespace CyberNet.Core
{
    [Serializable]
    public struct CardComponentAnimations
    {
        public Vector3 Positions;
        public Vector3 Scale;
        public Quaternion Rotate;
        public Sequence Sequence;
        public int SortingOrder;
    }
}