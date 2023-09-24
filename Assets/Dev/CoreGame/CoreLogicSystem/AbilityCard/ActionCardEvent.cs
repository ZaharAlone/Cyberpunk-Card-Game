using System;

namespace CyberNet.Core.AbilityCard
{
    public static class ActionCardEvent
    {
        //Считаем сколько ресурсов стало
        public static Action UpdateValueResourcePlayedCard;
        public static Action ClearActionView;
        
        //Делегаты для вызова/проверки определенных абилок
        public static Action CheckDiscardCard;
    }   
}
