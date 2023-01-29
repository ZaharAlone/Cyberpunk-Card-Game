using System;

namespace BoardGame.Core
{
    public static class InteractiveActionCard
    {
        public static Action<string> StartInteractiveCard;
        public static Action EndInteractiveCard;
        public static Action HideViewCard;
    }
}