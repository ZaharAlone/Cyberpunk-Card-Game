using ModulesFramework.Data.Enumerators;
using UnityEngine;
using System;

namespace BoardGame.Core
{
    public static class SortingCard
    {
        public static EntitiesEnumerable FirstSorting(int count, EntitiesEnumerable entities)
        {
            var sorting = Sorting(count);
            var index = 0;

            foreach (var entity in entities)
            {
                entity.AddComponent(new CardSortingIndexComponent { Index = sorting[index] });
                index++;
            }

            return entities;
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