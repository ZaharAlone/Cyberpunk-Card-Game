using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberNet.Core.ActionCard
{
    public static class ActionCardEvent
    {
        //Считаем сколько ресурсов стало
        public static System.Action UpdateValueResourcePlayedCard;
        public static System.Action ClearActionView;
        
        //Делегаты для вызова/проверки определенных абилок
        public static System.Action CheckDiscardCard;
    }   
}
