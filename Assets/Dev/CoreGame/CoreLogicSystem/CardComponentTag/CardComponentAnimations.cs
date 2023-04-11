using UnityEngine;
using DG.Tweening;

namespace BoardGame.Core
{
    public struct CardComponentAnimations
    {
        public Vector3 Positions;
        public Vector3 Scale;
        public Quaternion Rotate;
        public Sequence Sequence;
        public int SortingOrder;
    }
}