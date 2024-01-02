using System;
namespace CyberNet.Core.AI
{
    public static class AbilityAIAction
    {
        public static Action DiscardCardSelectPlayer;
        public static Action DiscardCardSelectCard;

        public static Action AddNoiseSelectPlayer;
        
        public static Action<string> MoveUnit;
        public static Func<ItemValue> CalculatePotentialMoveUnitAttack;
        public static Func<ItemValue> CalculatePotentialMoveUnit;
        public static Action<string> AddUnitMap;
        
        //Not working
        public static Action<string> DestroyNeutralUnit;
        public static Action<string> DestroyEnemyUnit;
        public static Action<string> SetIce;
        public static Action<string> DestroyIce;
    }
}