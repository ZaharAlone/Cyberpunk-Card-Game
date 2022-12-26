using ModulesFramework.Data.Enumerators;

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
    }
}