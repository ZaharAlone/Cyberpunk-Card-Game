using UnityEngine;

namespace BoardGame.Core
{
    public static class SetCardComponent
    {
        public static CardComponent Set(GameObject go, CardConfig stats, CardMono cardMono)
        {
            var component = new CardComponent
            {
                GUID = System.Guid.NewGuid().ToString(),
                Canvas = cardMono.Canvas,
                GO = go,
                Transform = go.transform,
                Stats = stats,
                CardMono = cardMono,
                Key = stats.Name,
                Nations = (CardNations)System.Enum.Parse(typeof(CardNations), stats.Nations),
                CyberpsychosisCount = stats.CyberpsychosisCount,
                Price = stats.Price,
                Ability_0 = stats.Ability_0,
                Ability_1 = stats.Ability_1,
            };

            component.CardMono.InteractiveCard.GUID = component.GUID;

            return component;
        }
    }
}