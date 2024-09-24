using System;
using ModulesFramework.Data;
using UnityEngine;

namespace CyberNet.Core
{
    public static class SetupCardAction
    {
        public static Func<CardData, Transform, bool, Entity> CreateCard;
        //Задать визуал карте, но не создавать её. Такие карты нужны к примеру для UI уничтожение карты
        public static Action<CardMono, string> SetViewCardNotInitToDeck;
    }
}