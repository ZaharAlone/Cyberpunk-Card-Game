using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CyberNet.Core.ActionCard;
using CyberNet.Core.UI;

namespace CyberNet.Core.Enemy
{
    [EcsSystem(typeof(VSAIModule))]
    public class EnemyAISystem : IPostRunEventSystem<EventUpdateRound>
    {
        private DataWorld _dataWorld;

        public void PostRunEvent(EventUpdateRound _)
        {
            var roundData = _dataWorld.OneData<RoundData>();

            if (roundData.CurrentPlayer == PlayerEnum.Player2)
                StartTurn();
        }

        private async void StartTurn()
        {
            Debug.Log("Enter Start Turn Enemy");
            await Task.Delay(1000);
            PlayAll();
            await Task.Delay(1000);
            SelectTradeCard();
            await Task.Delay(1000);
            ActionPlayerButtonEvent.ActionAttackBot?.Invoke();
            await Task.Delay(1000);
            ActionPlayerButtonEvent.ActionEndTurnBot?.Invoke();
        }

        private void PlayAll()
        {
            Debug.Log("Enemy Play All Card");
            var entities = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.Player == PlayerEnum.Player2)
                .With<CardHandComponent>().GetEntities();

            foreach (var entity in entities)
            {
                entity.RemoveComponent<CardHandComponent>();
                SelectionAbility(entity);
            }
            
            AnimationsMoveBoardCardAction.AnimationsMoveBoardCard?.Invoke();
        }

        private void SelectionAbility(Entity entity)
        {
            ref var cardComponent = ref entity.GetComponent<CardComponent>();

            if (cardComponent.Ability_1.AbilityType == AbilityType.None)
            {
                entity.AddComponent(new CardTableComponent {
                    SelectAbility = SelectAbilityEnum.Ability_0
                });
                return;
            }

            var valueAbility_0 = CalculateValueCardInterface(cardComponent.Ability_0);
            var valueAbility_1 = CalculateValueCardInterface(cardComponent.Ability_1);

            if (valueAbility_0 > valueAbility_1)
            {
                entity.AddComponent(new CardTableComponent {
                    SelectAbility = SelectAbilityEnum.Ability_0
                });
                Debug.LogError("Select Ability 0");
            }
            else
            {
                Debug.LogError("Select Ability 1");
                entity.AddComponent(new CardTableComponent {
                    SelectAbility = SelectAbilityEnum.Ability_1
                });
            }
        }

        private int CalculateValueCardInterface(AbilityCard abilityCard)
        {
            var value = 0;
            
            switch (abilityCard.AbilityType)
            {
                case AbilityType.Attack:
                    value = CalculateValueCardAction.AttackAction.Invoke(abilityCard.Count);
                    break;
                case AbilityType.Trade:
                    value = CalculateValueCardAction.TradeAction.Invoke(abilityCard.Count);
                    break;
                case AbilityType.DrawCard:
                    break;
                case AbilityType.DestroyCard:
                    value = CalculateValueCardAction.DestroyCardAction.Invoke();
                    break;
            }

            Debug.LogError($"Ability {abilityCard.AbilityType.ToString()} Value {value}");
            return value;
        }

        private void SelectTradeCard()
        {
            ref var actionData = ref _dataWorld.OneData<ActionCardData>();
            var tradePoint = actionData.TotalTrade - actionData.SpendTrade;
            
            if (tradePoint == 0)
                return;

            var availableTradeRowCardEntities = _dataWorld.Select<CardComponent>()
                .With<CardTradeRowComponent>()
                .Where<CardComponent>(card => card.Price <= tradePoint)
                .GetEntities();

            var scoresCard = new List<ScoreCardToBuy>();
            foreach (var cardEntity in availableTradeRowCardEntities)
            {
                ref var cardComponent = ref cardEntity.GetComponent<CardComponent>();
                var scoreCard = CalculateCardScore(cardComponent.Ability_0) + CalculateCardScore(cardComponent.Ability_1) + CalculateCardScore(cardComponent.Ability_2);
                scoreCard /= cardComponent.Price;
                Debug.LogError($"Trade row card name {cardComponent.Key} price {scoreCard}");
                scoresCard.Add(new ScoreCardToBuy { GUID = cardComponent.GUID, ScoreCard = scoreCard, Cost = cardComponent.Price});
            }

            var cardForPurchase = CalculateOptimalBuyCard.FindOptimalPurchase(scoresCard, tradePoint);
            PurchaseCard(cardForPurchase);
        }

        private float CalculateCardScore(AbilityCard abilityCard)
        {
            if (abilityCard.AbilityType == AbilityType.None)
                return 0f;
            
            var value = 0f;
            ref var configScoreCard = ref _dataWorld.OneData<BotConfigData>().BotScoreCard;
            var isFindConfig = configScoreCard.TryGetValue(abilityCard.AbilityType.ToString(), out var multValueAction);

            //TODO: Заглушка пока еще не все конфиги заданы для карт
            if (!isFindConfig)
                return value;
            switch (abilityCard.AbilityType)
            {
                case AbilityType.Attack:
                    value = multValueAction * abilityCard.Count;
                    break;
                case AbilityType.Trade:
                    value = multValueAction * abilityCard.Count;
                    break;
                case AbilityType.DrawCard:
                    value = multValueAction;
                    break;
                case AbilityType.DestroyCard:
                    value = multValueAction;
                    break;
                case AbilityType.CloneCard:
                    value = multValueAction;
                    break;
            }
            
            return value;
        }

        private void PurchaseCard(List<string> cardForPurchase)
        {
            ref var roundPlayer = ref _dataWorld.OneData<RoundData>().CurrentPlayer;
            ref var actionValue = ref _dataWorld.OneData<ActionCardData>();
            
            foreach (var purchaseCardGUID in cardForPurchase)
            {
                var cardEntity = _dataWorld.Select<CardComponent>()
                    .With<CardTradeRowComponent>()
                    .Where<CardComponent>(card => card.GUID == purchaseCardGUID)
                    .SelectFirstEntity();
                
                ref var componentCard = ref cardEntity.GetComponent<CardComponent>();
                actionValue.SpendTrade += componentCard.Price;
                cardEntity.RemoveComponent<CardTradeRowComponent>();
                
                componentCard.Player = roundPlayer;
                cardEntity.AddComponent(new CardMoveToDiscardComponent());
                
                Debug.LogError($"Buy card name {componentCard.Key}");
            }
            
            AnimationsMoveAtDiscardDeckAction.AnimationsMoveAtDiscardDeck?.Invoke();
            BoardGameUIAction.UpdateStatsPlayersCurrency?.Invoke();
            VFXCardInteractivAction.UpdateVFXCard?.Invoke();
            CardShopAction.CheckPoolShopCard?.Invoke();
        }
    }
}