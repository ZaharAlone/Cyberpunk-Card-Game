using System;

namespace CyberNet.Core.AbilityCard
{
    public static class AbilityPopupUISystemAction
    {
        public static Action<AbilityType, int, bool> OpenPopupAbilityTargetInfo;
        public static Action ClosePopup;
    }
}