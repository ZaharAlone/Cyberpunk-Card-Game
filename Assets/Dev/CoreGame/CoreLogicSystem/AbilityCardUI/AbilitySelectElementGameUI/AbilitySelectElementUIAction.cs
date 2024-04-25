using System;

namespace CyberNet.Core.AbilityCard
{
    public static class AbilitySelectElementUIAction
    {
        public static Action<AbilityType> SelectEnemyPlayer;
        public static Action CancelSelectPlayer;
        public static Action<AbilityType, int, bool> OpenSelectAbilityCard;
        public static Action<string> SelectElement;
        public static Action ClosePopup;
    }
}