using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using System.Collections.Generic;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.InteractiveCard;
using CyberNet.Core.Player;
using CyberNet.Core.UI;
using CyberNet.Global;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class CardShopSystem : IActivateSystem, IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void Activate()
        {
            CheckPoolShopCard();
        }

        public void PreInit()
        {
            CardShopAction.CheckPoolShopCard += CheckPoolShopCard;
            BoardGameUIAction.UpdateStatsPlayersCurrency += SelectCardFreeToBuy;
        }
        
        private void CheckPoolShopCard()
        {
            var boardGameData = _dataWorld.OneData<BoardGameData>();
            var countCardInShop = _dataWorld.Select<CardComponent>()
                                            .With<CardTradeRowComponent>()
                                            .Without<CardNeutralComponent>()
                                            .Count();

            if (countCardInShop == boardGameData.BoardGameRule.OpenCardInShop)
                return;

            var freeCell = GetFreeSlotInTradeRow();

            for (var i = 0; i < freeCell.Count; i++)
            {
                var entities = _dataWorld.Select<CardComponent>()
                                                     .With<CardTradeDeckComponent>()
                                                     .With<CardSortingIndexComponent>()
                                                     .GetEntities();

                var id = SortingCard.ChooseNearestCard(entities);
                AddTradeRowCard(id, freeCell[i]);
            }

            var playerComponent = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity().GetComponent<PlayerComponent>();

            if (playerComponent.playerOrAI == PlayerOrAI.Player)
            {
                _dataWorld.RiseEvent(new EventUpdateBoardCard());
                SelectCardFreeToBuy();   
            }
            else
            {
                ClearComponentInShop();
            }
        }

        private List<int> GetFreeSlotInTradeRow()
        {
            var entitiesCardInTradeRow = _dataWorld.Select<CardComponent>()
                                                   .With<CardTradeRowComponent>()
                                                   .Without<CardNeutralComponent>()
                                                   .GetEntities();

            var fullIndex = new List<int>();
            foreach (var entity in entitiesCardInTradeRow)
            {
                ref var component = ref entity.GetComponent<CardTradeRowComponent>();
                fullIndex.Add(component.Index);
            }

            var correctCell = new List<int> { 0, 1, 2, 3, 4 };

            foreach (var index in fullIndex)
                correctCell.Remove(index);

            return correctCell;
        }

        private void AddTradeRowCard(int entityId, int indexPositionCard)
        {
            ref var sizeCard = ref _dataWorld.OneData<BoardGameData>().BoardGameConfig.SizeCardInTraderow;
            var entity = _dataWorld.GetEntity(entityId);
            entity.RemoveComponent<CardTradeDeckComponent>();

            entity.AddComponent(new CardTradeRowComponent { Index = indexPositionCard});

            ref var cardComponent = ref entity.GetComponent<CardComponent>();
            cardComponent.CardMono.RectTransform.localScale = sizeCard;
            cardComponent.CardMono.ShowCard();
            cardComponent.CardMono.CardOnFace();
        }

        private void SelectCardFreeToBuy()
        {
            ClearComponentInShop();

            var entities = _dataWorld.Select<CardTradeRowComponent>().GetEntities();
            var action = _dataWorld.OneData<ActionCardData>();
            var tradePoint = action.TotalTrade - action.SpendTrade;

            foreach (var entity in entities)
            {
                ref var cardComponent = ref entity.GetComponent<CardComponent>();

                if (cardComponent.Stats.Price <= tradePoint)
                    entity.AddComponent(new CardFreeToBuyComponent());
            }
            
            VFXCardInteractiveAction.UpdateVFXCard?.Invoke();
        }

        private void ClearComponentInShop()
        {
            var entities = _dataWorld.Select<CardFreeToBuyComponent>().GetEntities();
            foreach (var entity in entities)
                entity.RemoveComponent<CardFreeToBuyComponent>();
        }

        public void Destroy()
        {
            CardShopAction.CheckPoolShopCard -= CheckPoolShopCard;
            BoardGameUIAction.UpdateStatsPlayersCurrency -= SelectCardFreeToBuy;
        }
    }
}