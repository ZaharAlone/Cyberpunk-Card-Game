using System;
using CyberNet.Core.City;

namespace CyberNet.Core.AbilityCard
{
    public static class AbilityCardAction
    {
        //Считаем сколько ресурсов стало
        public static Action UpdateValueResourcePlayedCard;
        public static Action ClearActionView;
        public static Action<int> SelectPlayer;
        
        //Делегаты вызова абилки
        public static Action AddResource;
        public static Action DiscardCard;
        public static Action CancelDiscardCard;
        public static Action PlayerDiscardCard;
        public static Action AddNoiseCard;
        
        public static Action<string> AddUnitMap;
        public static Action<string> MoveUnit;
        public static Action ConfimSelectElement;
    }   
}
