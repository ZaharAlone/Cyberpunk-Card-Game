using UnityEngine;

namespace BoardGame.Core
{
    public static class SetCardComponent
    {
        public static CardComponent Set(GameObject go, CardStats stats, CardMono cardMono)
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
                Price = stats.Price,
                Type = (CardType)System.Enum.Parse(typeof(CardType), stats.Type),
                Ability = stats.Ability,
                FractionsAbility = stats.FractionsAbility,
                DropAbility = stats.DropAbility
            };

            component.CardMono.InteractiveCard.GUID = component.GUID;

            return component;
        }
    }
}