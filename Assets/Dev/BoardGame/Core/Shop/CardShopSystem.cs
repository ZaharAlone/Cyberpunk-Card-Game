using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFrameworkUnity;
using System.Collections.Generic;
using UnityEngine;

namespace BoardGame.Core
{
    [EcsSystem(typeof(BoardGameModule))]
    public class CardShopSystem : IInitSystem
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            var boardGameData = _dataWorld.GetOneData<BoardGameData>().GetData();
            var countCardInShop = _dataWorld.Select<CardComponent>()
                             .With<CardInShopComponent>()
                             .Count();

            if (countCardInShop == boardGameData.BoardGameRule.OpenCardInShop)
                return;

            Entity selectEntity = new Entity();
            var minIndex = Mathf.Infinity;

            for (int i = countCardInShop; i < boardGameData.BoardGameRule.OpenCardInShop; i++)
            {
                var cardInDeckEntities = _dataWorld.Select<CardComponent>()
                                                     .With<CardInDeckComponent>()
                                                     .With<CardSortingIndexComponent>()
                                                     .GetEntities();

                foreach (var entity in cardInDeckEntities)
                {
                    ref var cardComponent = ref entity.GetComponent<CardSortingIndexComponent>();
                    if (cardComponent.Index <= minIndex)
                    {
                        minIndex = cardComponent.Index;
                        selectEntity = entity;
                    }
                }
                AddShopCard(selectEntity, countCardInShop);
                minIndex++;
                countCardInShop++;
            }
        }

        private void AddShopCard(Entity entity, int countCardInShop)
        {
            var boardGameData = _dataWorld.GetOneData<BoardGameData>().GetData();
            entity.RemoveComponent<CardInDeckComponent>();
            var pos = boardGameData.BoardGameConfig.PositionsShopFirstCard;
            pos.x += 20 + countCardInShop * 225;
            entity.AddComponent(new CardInShopComponent { Positions = pos });

            ref var cardComponent = ref entity.GetComponent<CardComponent>();
            cardComponent.Transform.position = pos;
            cardComponent.CardMono.CardOnFace();
        }

    }
}