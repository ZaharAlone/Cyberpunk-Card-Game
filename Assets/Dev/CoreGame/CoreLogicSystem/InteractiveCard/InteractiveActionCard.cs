using System;

namespace CyberNet.Core.InteractiveCard
{
    public static class InteractiveActionCard
    {
        public static Action<string> StartInteractiveCard;
        public static Action<string> FinishSelectAbilityCard;
        
        public static Action<string> SelectCardMap;
        public static Action<string> DeselectCardMap;
        
        public static Action EndInteractiveCard;
        public static Action ReturnAllCardInHand;
    }
}