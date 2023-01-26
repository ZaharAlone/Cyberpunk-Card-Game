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

        public void PostRunEvent(EventUpdateBoardCard _) => UpdateVFX();

        private void UpdateVFX()
        {
            Debug.Log("UPdateVFX");
            var entitiesCardInHand = _dataWorld.Select<CardComponent>().With<CardPlayerComponent>().With<CardInHandComponent>().GetEntities();
            //var entitiesCardInDeck = _dataWorld.Select<CardComponent>().With<CardPlayerComponent>().With<CardInDeckComponent>().GetEntities();
            //var entitiesCardInShop = _dataWorld.Select<CardComponent>().With<CardInShopComponent>().GetEntities();
            //var entitiesCardInDrop = _dataWorld.Select<CardComponent>().With<CardPlayerComponent>().With<CardInDropComponent>().GetEntities();
            //var actionValue = _dataWorld.OneData<ActionData>();
            //var valueTrade = actionValue.TotalTrade - actionValue.SpendTrade;


            foreach (var entity in entitiesCardInHand)
            {
                Debug.Log("Enter card in hand");
                ref var component = ref entity.GetComponent<CardComponent>().CardMono;
                component.SetStatusInteractiveVFX(true);
            }
            /*
            foreach (var entity in entitiesCardInDeck)
            {
                Debug.Log("Enter card in hand");
                ref var component = ref entity.GetComponent<CardComponent>().CardMono;
                component.SetStatusInteractiveVFX(false);
            }
            
            foreach (var entity in entitiesCardInDrop)
            {
                Debug.Log("Enter card in hand");
                ref var component = ref entity.GetComponent<CardComponent>().CardMono;
                component.SetStatusInteractiveVFX(false);
            }
            foreach (var entity in entitiesCardInShop)
            {
                Debug.Log("Enter card in hand");
                ref var component = ref entity.GetComponent<CardComponent>();
                if (component.Price <= valueTrade)
                    component.CardMono.SetStatusInteractiveVFX(true);
                else
                    component.CardMono.SetStatusInteractiveVFX(false);
            }
            */
        }
    }
}