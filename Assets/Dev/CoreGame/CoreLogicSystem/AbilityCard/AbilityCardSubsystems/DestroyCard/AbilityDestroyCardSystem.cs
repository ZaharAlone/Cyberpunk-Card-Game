using CyberNet.Core.AbilityCard.UI;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.AI.Ability;
using CyberNet.Core.UI;
using CyberNet.Global;
using CyberNet.Global.Config;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CyberNet.Core.AbilityCard.DestroyCard
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityDestroyCardSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AbilityCardAction.DestroyCardAbility += InitAbilityDestroyCard;
            DestroyCardAction.SelectCardToDestroy += DestroyCard;
            AbilityCardAction.CancelDestroyCard += CancelDestroyCard;
            DestroyCardAction.ForceCompleteDestroyCard += ForceCompleteDestroyCard;
        }

        private void InitAbilityDestroyCard(string guidCard)
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();

            if (roundData.playerOrAI != PlayerOrAI.Player)
                AbilityAIAction.DestroyCard?.Invoke(guidCard);
            else
                InitViewDestroyPanel(guidCard);
        }

        private void InitViewDestroyPanel(string guidCard)
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();
            
            _dataWorld.CreateOneData(new DestroyRowCardData{ DestroyCardInRow = new()});
            var countHandCard = SetViewCardInHand(guidCard, roundData.CurrentPlayerID);
            var countDiscardCard = SetViewCardDiscard(roundData.CurrentPlayerID);
            var countPlayZoneCard = SetViewCardInPlayZone(roundData.CurrentPlayerID);
            
            var boardGameUI = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;
            boardGameUI.TraderowMono.DisableTradeRow();

            if (countPlayZoneCard > 0)
                boardGameUI.DestroyCardUIMono.SelectCardInPlayZone();
            else if (countDiscardCard > 0)
                boardGameUI.DestroyCardUIMono.SelectCardInDiscard();
            else if (countHandCard > 0)
                boardGameUI.DestroyCardUIMono.SelectCardInHand();
            else
            {
                //TODO show not card for destroy popup
            }

            var cardAbilityComponent = _dataWorld.Select<CardComponent>()
                .With<AbilitySelectElementComponent>()
                .Where<CardComponent>(card => card.GUID == guidCard)
                .SelectFirstEntity()
                .GetComponent<AbilitySelectElementComponent>();

            var supportLocData = _dataWorld.OneData<BoardGameData>().SupportLocalize;

            if (cardAbilityComponent.AbilityCard.Count == 1)
            {
                boardGameUI.DestroyCardUIMono.EnablePanel(supportLocData.HeaderDestroyOneCard, supportLocData.DescrDestroyOneCard);
            }
            else
            {
                boardGameUI.DestroyCardUIMono.EnablePanel(supportLocData.HeaderDestroyManyCard, 
                    supportLocData.DescrDestroyManyCard, cardAbilityComponent.AbilityCard.Count.ToString());
            }
        }

        private int SetViewCardInHand(string guidCard, int playerID)
        {
            var queryCardInHand = _dataWorld.Select<CardComponent>()
                .With<CardHandComponent>()
                .Where<CardComponent>(card => card.GUID != guidCard && card.PlayerID == playerID);

            var entitiesCardInHand = queryCardInHand.GetEntities();
            foreach (var entity in entitiesCardInHand)
            {
                var cardComponent = entity.GetComponent<CardComponent>();
                AddCardInBar(cardComponent.Key, cardComponent.GUID, GroupDestroyCardEnum.Hand);
            }
            
            return queryCardInHand.Count();
        }

        private int SetViewCardDiscard(int playerID)
        {
            var queryCardsInDiscard = _dataWorld.Select<CardComponent>()
                .With<CardDiscardComponent>()
                .Where<CardComponent>(card => card.PlayerID == playerID);

            var entitiesCardsInDiscard = queryCardsInDiscard.GetEntities();
            foreach (var entity in entitiesCardsInDiscard)
            {
                var cardComponent = entity.GetComponent<CardComponent>();
                AddCardInBar(cardComponent.Key, cardComponent.GUID, GroupDestroyCardEnum.Discard);
            }

            return queryCardsInDiscard.Count();
        }
        
        private int SetViewCardInPlayZone(int playerID)
        {
            var queryCardsInTable = _dataWorld.Select<CardComponent>()
                .With<CardInTableComponent>()
                .Where<CardComponent>(card => card.PlayerID == playerID);

            var entitiesCardsInTable = queryCardsInTable.GetEntities();
            foreach (var entity in entitiesCardsInTable)
            {
                var cardComponent = entity.GetComponent<CardComponent>();
                AddCardInBar(cardComponent.Key, cardComponent.GUID, GroupDestroyCardEnum.PlayZone);
            }

            return queryCardsInTable.Count();
        }
        
        private void AddCardInBar(string keyCard, string guid, GroupDestroyCardEnum targetGroupCardEnum)
        {
            ref var destroyRowData = ref _dataWorld.OneData<DestroyRowCardData>();
            var cardSupport = _dataWorld.OneData<BoardGameData>().BoardGameConfig.CardDestroy;
            var destroyCardUIMono = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.DestroyCardUIMono;
            var cardMono = destroyCardUIMono.CreateNewCard(cardSupport, targetGroupCardEnum);

            var interactiveDestroyCard = cardMono.gameObject.GetComponent<InteractiveDestroyCardMono>();
            interactiveDestroyCard.SetGUID(guid);
            SetupCardAction.SetViewCardNotInitToDeck?.Invoke(cardMono, keyCard);

            var destroyElement = new DestroyCardInRow {
                CardMono = cardMono, InteractiveDestroyCardMono = interactiveDestroyCard
            };
            destroyRowData.DestroyCardInRow.Add(guid, destroyElement);
        }
        
        private void DestroyCard(string guidCardToDestroy)
        {
            var destroyCardAbilityEntity = _dataWorld.Select<AbilityCardDestroyCardComponent>().SelectFirstEntity();
            ref var destroyCardAbilityComponent = ref destroyCardAbilityEntity.GetComponent<AbilityCardDestroyCardComponent>();
            var abilitySelectAbilityCard = destroyCardAbilityEntity.GetComponent<AbilitySelectElementComponent>();

            destroyCardAbilityComponent.CountUseElement++;

            var uiDestroyRow = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.DestroyCardUIMono;
            
            if (destroyCardAbilityComponent.CountUseElement == abilitySelectAbilityCard.AbilityCard.Count)
            {
                uiDestroyRow.DisablePanel();
                AbilityInputButtonUIAction.HideInputUIButton?.Invoke();
                DestroyCardAction.EndAnimationsDestroyCurrentCard += EndPlayingAbility;
            }
            else
            {
                uiDestroyRow.SetEnableButtonComplete(true);
            }
            
            DestroySelectedCard(guidCardToDestroy);
        }

        private void DestroySelectedCard(string guidCard)
        {
            var destroyCardRow = _dataWorld.OneData<DestroyRowCardData>().DestroyCardInRow;
            destroyCardRow.TryGetValue(guidCard, out var cardElement);
            cardElement.InteractiveDestroyCardMono.AnimationsDestroy();
            
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
        
        private void EndPlayingAbility()
        {
            DestroyCardAction.EndAnimationsDestroyCurrentCard -= EndPlayingAbility;

            var boardGameUI = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;
            boardGameUI.DestroyCardUIMono.ClearCards();
            boardGameUI.TraderowMono.EnableTradeRow();
    
            var entityCard = _dataWorld.Select<CardComponent>()
                .With<AbilitySelectElementComponent>()
                .With<AbilityCardDestroyCardComponent>()
                .SelectFirstEntity();

            var cardComponent = entityCard.GetComponent<CardComponent>();
            entityCard.RemoveComponent<AbilityCardDestroyCardComponent>();

            AbilityCardAction.CompletePlayingAbilityCard?.Invoke(cardComponent.GUID);
            
            ref var destroyRowData = ref _dataWorld.OneData<DestroyRowCardData>();
            destroyRowData.DestroyCardInRow.Clear();
        }
        
        private void CancelDestroyCard(string guidCard)
        {
            var boardGameUI = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;
            boardGameUI.DestroyCardUIMono.DisablePanel();
            boardGameUI.DestroyCardUIMono.ClearCards();
            boardGameUI.TraderowMono.EnableTradeRow();

            ref var destroyRowData = ref _dataWorld.OneData<DestroyRowCardData>();
            destroyRowData.DestroyCardInRow.Clear();
        }

        private void ForceCompleteDestroyCard()
        {
            EndPlayingAbility();
        }

        public void Destroy()
        {
            AbilityCardAction.DestroyCardAbility -= InitAbilityDestroyCard;
            DestroyCardAction.SelectCardToDestroy -= DestroyCard;
            AbilityCardAction.CancelDestroyCard -= CancelDestroyCard;
            DestroyCardAction.ForceCompleteDestroyCard -= ForceCompleteDestroyCard;
        }
    }
}