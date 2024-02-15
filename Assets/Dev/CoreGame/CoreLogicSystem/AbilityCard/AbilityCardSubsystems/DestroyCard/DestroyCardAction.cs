using System;

namespace CyberNet.Core.AbilityCard.DestroyCard
{
    public static class DestroyCardAction
    {
        public static Action<string> SelectCard;
        public static Action<string> DeselectCard;
        public static Action<string> StartMoveCard;
        public static Action<string> EndMoveCard;

        public static Action<string> SelectCardToDestroy;
    }
}