using UnityEngine;

namespace CyberNet.Core
{
    public static class SetCardComponent
    {
        public static CardComponent Set(GameObject go, CardConfigJson stats, CardMono cardMono)
        {
            var component = new CardComponent
            {
                GUID = System.Guid.NewGuid().ToString(),
                Canvas = cardMono.Canvas,
                GO = go,
                RectTransform = cardMono.RectTransform,
                Stats = stats,
                CardMono = cardMono,
                ValueLeftPoint = stats.ValueLeftPoint,
                ValueRightPoint = stats.ValueRightPoint,
                Key = stats.Name,
                Price = stats.Price,
                Ability_0 = stats.Ability_0,
                Ability_1 = stats.Ability_1,
            };

            component.CardMono.InteractiveCard.GUID = component.GUID;

            return component;
        }
    }
}