using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using UnityEngine;

namespace BoardGame.Core
{
    [EcsSystem(typeof(BoardGameModule))]
    public class CardShopSystem : IActivateSystem, IPostRunEventSystem<EventBoardGameUpdate>
    {
        private DataWorld _dataWorld;

        public void Activate()
        {
            CheckPoolShopCard();
        }

        private void CheckPoolShopCard()
        {
            var boardGameData = _dataWorld.OneData<BoardGameData>();
            var countCardInShop = _dataWorld.Select<CardComponent>()
                             .With<CardTradeRowComponent>()
                             .Count();

            if (countCardInShop == boardGameData.BoardGameRule.OpenCardInShop)
                return;

            for (var i = countCardInShop; i < boardGameData.BoardGameRule.OpenCardInShop; i++)
            {
                var entities = _dataWorld.Select<CardComponent>()
                                                     .With<CardDeckComponent>()
                                                     .With<CardSortingIndexComponent>()
                                                     .Without<CardPlayerComponent>()
                                                     .Without<CardEnemyComponent>()
                                                     .GetEntities();

                var id = SortingCard.SelectCard(entities);
                Debug.Log(id);
                AddShopCard(id, countCardInShop);
                countCardInShop++;
            }
        }

        private void AddShopCard(int entityId, int countCardInShop)
        {
            var entity = _dataWorld.GetEntity(entityId);
            var boardGameData = _dataWorld.OneData<BoardGameData>();
            entity.RemoveComponent<CardDeckComponent>();
            var pos = boardGameData.BoardGameConfig.PositionsShopFirstCard;
            pos.x += 20 + countCardInShop * 225;
            entity.AddComponent(new CardTradeRowComponent { Positions = pos });

            ref var cardComponent = ref entity.GetComponent<CardComponent>();
            cardComponent.Transform.position = pos;
            cardComponent.CardMono.CardOnFace();
        }

        public void PostRunEvent(EventBoardGameUpdate _)
        {
            CheckPoolShopCard();
            SelectCardFreeToBuy();
        }

        private void SelectCardFreeToBuy()
        {
            ClearComponentInShop();

            var enteties = _dataWorld.Select<CardTradeRowComponent>().GetEntities();
            var action = _dataWorld.OneData<ActionData>();
            var tradePoint = action.TotalTrade - action.SpendTrade;

            foreach (var entity in enteties)
            {
                ref var cardComponent = ref entity.GetComponent<CardComponent>();

                if (cardComponent.Stats.Price <= tradePoint)
                    entity.AddComponent(new CardFreeToBuyComponent());
            }
        }

        private void ClearComponentInShop()
        {
            var enteties = _dataWorld.Select<CardFreeToBuyComponent>().GetEntities();
            foreach (var entity in enteties)
                entity.RemoveComponent<CardFreeToBuyComponent>();
        }
    }
}