using System;

namespace BoardGame.Core
{
    public static class InteractiveActionCard
    {
        public static Action<string> StartInteractiveCard;
        public static Action<string> SelectCard;
        public static Action<string> DeselectCard;
        public static Action EndInteractiveCard;
        public static Action ReturnAllCardInHand;
    }
}