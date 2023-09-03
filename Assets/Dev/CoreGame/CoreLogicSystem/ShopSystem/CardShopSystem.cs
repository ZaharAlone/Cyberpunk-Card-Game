using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using System.Collections.Generic;
using CyberNet.Core.ActionCard;
using UnityEngine;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class CardShopSystem : IActivateSystem, IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void Activate()
        {
            CheckPoolShopCard();
        }

        public void PreInit()
        {
            CardShopAction.CheckPoolShopCard += CheckPoolShopCard;
            CardShopAction.SelectCardFreeToBuy += SelectCardFreeToBuy;
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

            _dataWorld.RiseEvent(new EventUpdateBoardCard());
            SelectCardFreeToBuy();
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

            //var pos = new Vector2(Screen.resolutions.Length/2 - 360, 200);
            //pos.x += 20 + indexPositionCard * 224;
            entity.AddComponent(new CardTradeRowComponent { Index = indexPositionCard/*, Positions = pos*/ });

            ref var cardComponent = ref entity.GetComponent<CardComponent>();
            //cardComponent.RectTransform.position = pos;
            cardComponent.CardMono.RectTransform.localScale = sizeCard;
            cardComponent.CardMono.ShowCard();
            cardComponent.CardMono.CardOnFace();
        }

        private void SelectCardFreeToBuy()
        {
            ClearComponentInShop();

            var enteties = _dataWorld.Select<CardTradeRowComponent>().GetEntities();
            var action = _dataWorld.OneData<ActionCardData>();
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