using ModulesFramework.Data.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CyberNet.Core
{
    public static class SortingCard
    {
        public static List<CardData> SortingDeckCards(List<CardData> Cards)
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
        
        public static List<CardData> SortingDeckCardsForTutorial(List<CardData> Cards)
        {
            for (int i = 0; i < Cards.Count; i++)
            {
                Cards[i] = CardSetPosition(Cards[i], -1);
            }

            var countCardSetPositions = 0;
            var countHunter = 0;
            
            while (countCardSetPositions < Cards.Count)
            {
                for (int i = 0; i < Cards.Count; i++)
                {
                    if (Cards[i].IDPositions != -1)
                        continue;
                    
                    if (Cards[i].CardName == "neutral_hunter" && countHunter < 2)
                    {
                        Cards[i] = CardSetPosition(Cards[i], countCardSetPositions);
                        countCardSetPositions++;
                        countHunter++;
                        break;
                    }

                    if (Cards[i].CardName != "neutral_hunter" && countHunter == 2)
                    {
                        Cards[i] = CardSetPosition(Cards[i], countCardSetPositions);
                        countCardSetPositions++;
                    }
                }
            }
            
            return Cards;
        }
        
        private static CardData CardSetPosition(CardData card, int index)
        {
            card.IDPositions = index;
            return card;
        }

        public static int[] Sorting(int count)
        {
            var sorting = new int[count];
            for (int i = 0; i < sorting.Length; i++)
                sorting[i] = i;
            
            var random = new System.Random();
            var n = count;
            
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                var value = sorting[k];
                sorting[k] = sorting[n];
                sorting[n] = value;
            }

            return sorting;
        }

        public static int ChooseNearestCard(EntitiesEnumerable cardEntities)
        {
            var id = 0;
            var minIndex = 500;
            
            foreach (var entity in cardEntities)
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