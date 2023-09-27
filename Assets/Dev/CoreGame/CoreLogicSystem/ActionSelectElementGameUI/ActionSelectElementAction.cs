using System;

namespace CyberNet.Core.AbilityCard
{
    public static class ActionSelectElementAction
    {
        public static Action CloseWindowAbilitySelectCard;
        public static Action<AbilityType> SelectEnemyPlayer;
        public static Action SelectCard;
        public static Action OpenSelectAbilityCard;
    }
}