using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.AI;
using CyberNet.Core.UI;
using CyberNet.Global;
using Object = UnityEngine.Object;

namespace CyberNet.Core.AbilityCard.DestroyCard
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityDestroyCardSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AbilityCardAction.DestroyCardAbility += DestroyCard;
            DestroyCardAction.SelectCardToDestroy += SelectCardToDestroy;
            AbilityCardAction.CancelDestroyCard += CancelDestroyCard;
        }

        private void DestroyCard(string guidCard)
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();

            if (roundData.playerOrAI != PlayerOrAI.Player)
            {
                AbilityAIAction.AddUnitMap?.Invoke(guidCard);
                return;
            }
            
            roundData.PauseInteractive = true;
            var entityCard = _dataWorld.Select<CardComponent>()
                .With<AbilitySelectElementComponent>().
                SelectFirstEntity();
            var targetCardGUID = entityCard.GetComponent<CardComponent>().GUID;

            var playerID = roundData.CurrentPlayerID;
            var entitiesCardInHand = _dataWorld.Select<CardComponent>()
                .With<CardHandComponent>()
                .Where<CardComponent>(card => card.GUID != targetCardGUID && card.PlayerID == playerID)
                .GetEntities();
            
            var entitiesCardsInTable = _dataWorld.Select<CardComponent>()
                .With<CardInTableComponent>()
                .Where<CardComponent>(card => card.PlayerID == playerID)
                .GetEntities();
            
            var entitiesCardsInDiscard = _dataWorld.Select<CardComponent>()
                .With<CardDiscardComponent>()
                .Where<CardComponent>(card => card.PlayerID == playerID)
                .GetEntities();
            
            _dataWorld.CreateOneData(new DestroyRowCardData{ DestroyCardInRow = new()});
            
            foreach (var entity in entitiesCardInHand)
            {
                var cardComponent = entity.GetComponent<CardComponent>();
                AddCardInBar(cardComponent.Key, cardComponent.GUID, true);
            }
            
            foreach (var entity in entitiesCardsInTable)
            {
                var cardComponent = entity.GetComponent<CardComponent>();
                AddCardInBar(cardComponent.Key, cardComponent.GUID, false);
            }
            
            foreach (var entity in entitiesCardsInDiscard)
            {
                var cardComponent = entity.GetComponent<CardComponent>();
                AddCardInBar(cardComponent.Key, cardComponent.GUID, false);
            }
            
            var destroyCardUIMono = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.DestroyCardUIMono;
            destroyCardUIMono.EnablePanel();
        }
        
        private void AddCardInBar(string keyCard, string guid, bool isHand)
        {
            ref var destroyRowData = ref _dataWorld.OneData<DestroyRowCardData>();
            var cardSupport = _dataWorld.OneData<BoardGameData>().BoardGameConfig.CardDestroy;
            var destroyCardUIMono = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.DestroyCardUIMono;
            var cardMono = destroyCardUIMono.CreateNewCard(cardSupport);

            var interactiveDestroyCard = cardMono.gameObject.GetComponent<InteractiveDestroyCardMono>();
            interactiveDestroyCard.SetGUID(guid);
            interactiveDestroyCard.SetIconsIsHand(isHand);
            SetupCardAction.SetViewCardNotInit?.Invoke(cardMono, keyCard);

            var destroyElement = new DestroyCardInRow {
                CardMono = cardMono, InteractiveDestroyCardMono = interactiveDestroyCard
            };
            destroyRowData.DestroyCardInRow.Add(guid, destroyElement);
        }
        
        private void SelectCardToDestroy(string guidCard)
        {
            var selectCardEntity = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == guidCard)
                .SelectFirstEntity();

            var isCardHand = selectCardEntity.HasComponent<CardHandComponent>();

            var cardComponent = selectCardEntity.GetComponent<CardComponent>();
            Object.Destroy(cardComponent.CardMono.gameObject);
            selectCardEntity.Destroy();

            if (isCardHand)
            {
                //Update hand row
            }
        }

        private void CancelDestroyCard(string guidCard)
        {
            
        }

        public void Destroy()
        {
            AbilityCardAction.DestroyCardAbility -= DestroyCard;
            AbilityCardAction.CancelDestroyCard -= CancelDestroyCard;
        }
    }
}