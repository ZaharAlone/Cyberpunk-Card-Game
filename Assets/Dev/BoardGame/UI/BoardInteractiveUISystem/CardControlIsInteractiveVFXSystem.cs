using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using ModulesFrameworkUnity;
using System.Collections.Generic;
using UnityEngine;

namespace BoardGame.Core.UI
{
    [EcsSystem(typeof(BoardGameModule))]
    public class CardControlIsInteractiveVFXSystem : IPostRunEventSystem<EventUpdateBoardCard>
    {
        private DataWorld _dataWorld;

        public void PostRunEvent(EventUpdateBoardCard _)
        {
            var roundData = _dataWorld.OneData<RoundData>();
            if (roundData.CurrentPlayer == PlayerEnum.Player)
                UpdateVFX();
        }

        private void UpdateVFX()
        {
            var entitiesCardInHand = _dataWorld.Select<CardComponent>().With<CardPlayerComponent>().With<CardHandComponent>().GetEntities();
            var entitiesCardInDeck = _dataWorld.Select<CardComponent>().With<CardPlayerComponent>().With<CardDeckComponent>().GetEntities();
            var entitiesCardInDrop = _dataWorld.Select<CardComponent>().With<CardPlayerComponent>().With<CardDiscardComponent>().GetEntities();
            var entitiesCardInShop = _dataWorld.Select<CardComponent>().With<CardTradeRowComponent>().GetEntities();
            var actionValue = _dataWorld.OneData<ActionData>();
            var valueTrade = actionValue.TotalTrade - actionValue.SpendTrade;

            foreach (var entity in entitiesCardInHand)
            {
                ref var component = ref entity.GetComponent<CardComponent>().CardMono;
                component.SetStatusInteractiveVFX(true);
            }
            
            foreach (var entity in entitiesCardInDeck)
            {
                ref var component = ref entity.GetComponent<CardComponent>().CardMono;
                component.SetStatusInteractiveVFX(false);
            }
            
            foreach (var entity in entitiesCardInDrop)
            {
                ref var component = ref entity.GetComponent<CardComponent>().CardMono;
                component.SetStatusInteractiveVFX(false);
            }
            
            foreach (var entity in entitiesCardInShop)
            {
                ref var component = ref entity.GetComponent<CardComponent>();
                if (component.Price <= valueTrade)
                    component.CardMono.SetStatusInteractiveVFX(true);
                else
                    component.CardMono.SetStatusInteractiveVFX(false);
            }
        }
    }
}