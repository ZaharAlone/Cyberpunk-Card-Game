using System;
using UnityEngine;

namespace CyberNet.Core.AbilityCard.DestroyCard
{
    public static class DestroyCardAction
    {
        public static Action<string> SelectCard;
        public static Action<string> DeselectCard;
        public static Action<string> StartMoveCard;
        public static Action<string> EndMoveCard;

        public static Action<string> SelectCardToDestroy;
        public static Action EndAnimationsDestroyCurrentCard;

        public static Action ForceCompleteDestroyCard;
    }
}