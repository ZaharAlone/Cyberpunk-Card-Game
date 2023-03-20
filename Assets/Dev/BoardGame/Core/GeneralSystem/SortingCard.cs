using ModulesFramework.Data.Enumerators;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace BoardGame.Core
{
    public static class SortingCard
    {
        public static List<PlaceCard> SortingDeckCards(List<PlaceCard> Cards)
        {
            var sorting = Sorting(Cards.Count);
            for (int i = 0; i < Cards.Count; i++)
            {
                var card = Cards[i];
                card.IDPositions = sorting[i];
                Cards[i] = card;
            }

            return Cards;
        }

        public static int[] Sorting(int count)
        {
            var sorting = new int[count];
            for (int i = 0; i < sorting.Length; i++)
                sorting[i] = i;

            var random = RandomSystem.RandomTime();

            for (var i = 0; i < count; i++)
            {
                var newPos = random.Next(0, count);
                var tempIndex = sorting[i];

                sorting[i] = sorting[newPos];
                sorting[newPos] = tempIndex;

                if (i % (count / 4f) == 0)
                    random = RandomSystem.RandomShift(count);
            }

            return sorting;
        }

        public static int SelectCard(EntitiesEnumerable entities)
        {
            var id = 0;
            var minIndex = Mathf.Infinity;
            foreach (var entity in entities)
            {
                ref var cardComponent = ref entity.GetComponent<CardSortingIndexComponent>();
                if (cardComponent.Index < minIndex)
                {
                    minIndex = cardComponent.Index;
                    id = entity.Id;
                }
            }
            return id;
        }
    }
}