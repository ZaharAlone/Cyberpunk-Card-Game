using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace CyberNet.Core.AbilityCard.DestroyCard
{
    public struct DestroyRowCardData
    {
        public Dictionary<string, DestroyCardInRow> DestroyCardInRow;
    }

    public struct DestroyCardInRow
    {
        public CardMono CardMono;
        public InteractiveDestroyCardMono InteractiveDestroyCardMono;
        
        //Animations element
        public Sequence Sequence;
        public Vector2 StartPosition;
        public Vector3 StartScale;
    }
}