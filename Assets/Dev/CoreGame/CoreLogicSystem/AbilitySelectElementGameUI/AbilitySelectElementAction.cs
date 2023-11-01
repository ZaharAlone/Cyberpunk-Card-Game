using System;

namespace CyberNet.Core.AbilityCard
{
    public static class AbilitySelectElementAction
    {
        public static Action CancelSelect;
        public static Action<AbilityType> SelectEnemyPlayer;
        public static Action CancelSelectPlayer;
        public static Action ConfimSelect;
        public static Action<AbilityType, int, bool> OpenSelectAbilityCard;
        public static Action<string> SelectElement;
    }
}