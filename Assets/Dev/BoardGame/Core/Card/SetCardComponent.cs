using UnityEngine;

namespace BoardGame.Core
{
    public static class SetCardComponent
    {
        public static CardComponent Set(GameObject go, CardStats stats)
        {
            var component = new CardComponent
            {
                GO = go,
                Transform = go.transform,
                Stats = stats,
                Key = stats.Name,
                Nations = (CardNations)System.Enum.Parse(typeof(CardNations), stats.Nations),
                Price = stats.Price,
                Type = (CardType)System.Enum.Parse(typeof(CardType), stats.Type),
                Ability = stats.Ability,
                FractionsAbility = stats.FractionsAbility,
                DropAbility = stats.DropAbility
            };

            return component;
        }
    }
}