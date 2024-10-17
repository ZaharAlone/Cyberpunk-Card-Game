using System;

namespace CyberNet.Core.AbilityCard
{
    public static class AbilityCardAction
    {
        //Считаем сколько ресурсов стало
        public static Action UpdateValueResourcePlayedCard;
        public static Action ClearActionView;
        public static Action CancelAbility;
        public static Action<string> CompletePlayingAbilityCard;
        
        //Делегаты вызова абилки
        public static Action<string> AddTradePoint;
        public static Action<string> DiscardCard;
        public static Action CancelDiscardCard;
        public static Action PlayerDiscardCard;
        public static Action<string> AddNoiseCard;
        public static Action CancelAddNoiseCard;
        
        public static Action<string> DestroyCardAbility;
        public static Action<string> CancelDestroyCard;
        
        public static Action<string> AbilityAddUnitMap;
        public static Action<string> CancelAddUnitMap;
        
        public static Action<string> MoveUnit;
        public static Action<string> CancelMoveUnit;
        
        public static Action<string> DestroyNeutralUnit;
        public static Action<string> DestroyEnemyUnit;
        public static Action<string> SetIce;
        public static Action<string> DestroyIce;
        public static Action<string> SwitchEnemyUnitMap;
        public static Action<string> SwitchNeutralUnitMap;
        public static Action ConfirmSelectElement;
        
        //Arena ability
        public static Action<string> HeadShot;
        public static Action<string> ThrowGrenade;

        //Вспомогательные делегаты, общие для систем
        public static Action<string> AddTowerUnit;
        public static Action CurrentAbilityEndPlaying;
        public static Action<string> ShiftUpCard;
    }   
}
