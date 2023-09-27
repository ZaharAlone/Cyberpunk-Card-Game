using System;

namespace CyberNet.Core.AbilityCard
{
    public static class AbilityCardAction
    {
        //Считаем сколько ресурсов стало
        public static Action UpdateValueResourcePlayedCard;
        public static Action ClearActionView;
        
        //Делегаты вызова абилки
        public static Action AddResource;
        public static Action DiscardCard;
        public static Action AddNoiseCard;
    }   
}
