using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace BoardGame.Core
{
    [EcsSystem(typeof(BoardGameModule))]
    public class CardShopSystem : IInitSystem
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            var boardGameData = _dataWorld.OneData<BoardGameData>();
            var countCardInShop = _dataWorld.Select<CardComponent>()
                             .With<CardInShopComponent>()
                             .Count();

            if (countCardInShop == boardGameData.BoardGameRule.OpenCardInShop)
                return;

            for (var i = countCardInShop; i < boardGameData.BoardGameRule.OpenCardInShop; i++)
            {
                var entities = _dataWorld.Select<CardComponent>()
                                                     .With<CardInDeckComponent>()
                                                     .With<CardSortingIndexComponent>()
                                                     .GetEntities();

                var id = SortingCard.SelectCard(entities);
                AddShopCard(id, countCardInShop);
                countCardInShop++;
            }
        }

        private void AddShopCard(int entityId, int countCardInShop)
        {
            var entity = _dataWorld.GetEntity(entityId);
            var boardGameData = _dataWorld.OneData<BoardGameData>();
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