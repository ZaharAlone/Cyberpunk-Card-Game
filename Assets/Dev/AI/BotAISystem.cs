using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System.Collections.Generic;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.AbilityCard.DiscardCard;
using CyberNet.Core.AI.Ability;
using CyberNet.Core.Map;
using CyberNet.Core.EnemyTurnView;
using CyberNet.Core.Player;
using CyberNet.Core.SelectFirstBase;
using CyberNet.Core.UI;
using CyberNet.Core.UI.CorePopup;
using CyberNet.Global;

namespace CyberNet.Core.AI
{
    [EcsSystem(typeof(CoreModule))]
    public class BotAISystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        private const float timeWaitActionBot = 0.7f;
        private const float timeWaitPlayingCard = 0.35f;
        
        public void PreInit()
        {
            RoundAction.StartTurnAI += StartTurn;
        }
        
        /// <summary>
        /// Начинаем раунд AI
        /// Проверяем есть ли база, если нет ставим
        /// Проверяем должны ли мы сбросить карты?
        /// </summary>
        private void StartTurn()
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();
            roundData.PauseInteractive = true;
            CoreElementInfoPopupAction.ClosePopupCard?.Invoke();
            
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();

            if (playerEntity.HasComponent<PlayerNotInstallFirstBaseComponent>())
                SelectFirstBase();

            if (playerEntity.HasComponent<PlayerEffectDiscardCardComponent>())
                DiscardCard();
            else
                StartTurnBot();
        }
        
        // Выбираем стартовую базу
        private void SelectFirstBase()
        {
            var firstBaseEntities = _dataWorld.Select<DistrictComponent>()
                .With<FirstBasePlayerComponent>()
                .GetEntities();
            
            var countFirstBase = _dataWorld.Select<DistrictComponent>()
                .With<FirstBasePlayerComponent>()
                .Count();

            var indexBase = Random.Range(0, countFirstBase);

            var counter = 0;
            foreach (var entityFirstBase in firstBaseEntities)
            {
                if (counter == indexBase)
                {
                    ref var towerComponent = ref entityFirstBase.GetComponent<DistrictComponent>();
                    SelectFirstBaseAction.SelectBase?.Invoke(towerComponent.GUID);
                    break;
                }
                
                counter++;
            }
        }

        private void DiscardCard()
        {
            //logic discard card
            AbilityAIAction.EndDiscardCard += EndDiscardCard;
            AbilityAIAction.DiscardCardSelectCard?.Invoke();
        }

        private void EndDiscardCard()
        {
            AbilityAIAction.EndDiscardCard -= EndDiscardCard;
            StartTurnBot();
        }

        // Начинаем ход бота
        private void StartTurnBot()
        {
            CityAction.UpdatePresencePlayerInCity?.Invoke();
            PlayCard();
        }

        private void PlayCard()
        {
            BotAIAction.ContinuePlayingCards -= PlayCard;
            
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;
            var countCard = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.PlayerID == currentPlayerID)
                .With<CardHandComponent>()
                .Count();

            if (countCard == 0)
            {
                var timeEntity = _dataWorld.NewEntity();
                timeEntity.AddComponent(new TimeComponent {
                    Time = timeWaitActionBot, Action = () => EndPlayingCards()
                });
                
                Debug.Log($"end playing card ai, player id {currentPlayerID}");

                return;
            }
            
            var selectEntityPlayCard = FindPriorityCardPlay();
                
            selectEntityPlayCard.RemoveComponent<CardHandComponent>();
            var selectAbilityType = SelectionAbility(selectEntityPlayCard);
            AbilityCardAction.UpdateValueResourcePlayedCard?.Invoke();
            var cardKey = selectEntityPlayCard.GetComponent<CardComponent>().Key;
            EnemyTurnViewUIAction.ShowViewEnemyCardCurrentPlayer?.Invoke(EnemyTurnActionType.PlayingCard, cardKey);

            // Если абилка - передвижение юнита, значит прерываем цикл разыгрывания карт и ждем окончания битвы.
            if (selectAbilityType == AbilityType.UnitMove)
            {
                BotAIAction.ContinuePlayingCards += PlayCard;
                Debug.Log($"wait ai playing move unit, player id {currentPlayerID}");
            }
            else
            {
                var timeEntity = _dataWorld.NewEntity();
                timeEntity.AddComponent(new TimeComponent {
                    Time = timeWaitPlayingCard, Action = () => PlayCard()
                });
                Debug.Log($"wait ai playing next card, player id {currentPlayerID}");
            }
        }
        
        private void EndPlayingCards()
        {
            SelectingCardToPurchase();
            
            _dataWorld.NewEntity().AddComponent(new TimeComponent {
                Time = timeWaitActionBot, Action = () => ActionPlayerButtonEvent.ActionEndTurnBot?.Invoke()
            });
        }
        
        //Ищем какую карту стоит разыграть в первую очередь
        private Entity FindPriorityCardPlay()
        {
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;
            var entities = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.PlayerID == currentPlayerID)
                .With<CardHandComponent>().GetEntities();
            
            //Check card add unit
            foreach (var entity in entities)
            {
                var cardComponent = entity.GetComponent<CardComponent>();

                if (cardComponent.Ability_0.AbilityType == AbilityType.AddUnit
                    || cardComponent.Ability_0.AbilityType == AbilityType.DrawCard
                    && cardComponent.Ability_1.AbilityType == AbilityType.None)
                {
                    return entity;
                }
            }

            foreach (var entity in entities)
            {
                var cardComponent = entity.GetComponent<CardComponent>();

                if (cardComponent.Ability_0.AbilityType != AbilityType.Trade
                    && cardComponent.Ability_0.AbilityType != AbilityType.DestroyCard)
                {
                    return entity;
                }
            }
            
            foreach (var entity in entities)
            {
                return entity;
            }
            
            Debug.LogError("Not find current ability");
            return new Entity();
        }

        private AbilityType SelectionAbility(Entity entity)
        {
            ref var cardComponent = ref entity.GetComponent<CardComponent>();

            if (cardComponent.Ability_1.AbilityType == AbilityType.None)
            {
                entity.AddComponent(new CardAbilitySelectionCompletedComponent {
                    SelectAbility = SelectAbilityEnum.Ability_0
                });
                return cardComponent.Ability_0.AbilityType;
            }

            var valueAbility_0 = CalculateValueCardAction.CalculateValueCardAbility.Invoke(cardComponent.Ability_0);
            var valueAbility_1 = CalculateValueCardAction.CalculateValueCardAbility.Invoke(cardComponent.Ability_1);

            //Temp for debug
            DebugLogicSelectAbility(cardComponent, valueAbility_0, valueAbility_1);
            
            if (valueAbility_0 > valueAbility_1)
            {
                entity.AddComponent(new CardAbilitySelectionCompletedComponent {
                    SelectAbility = SelectAbilityEnum.Ability_0
                });

                return cardComponent.Ability_0.AbilityType;
            }
            else
            {
                entity.AddComponent(new CardAbilitySelectionCompletedComponent {
                    SelectAbility = SelectAbilityEnum.Ability_1
                });

                return cardComponent.Ability_1.AbilityType;
            }
        }

        private void DebugLogicSelectAbility(CardComponent cardComponent, int valueAbility_0, int valueAbility_1)
        {
            //Temp for debug
            #if UNITY_EDITOR
            
            var targetPlayerID = cardComponent.PlayerID;
            var playerName = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player => player.PlayerID == targetPlayerID)
                .SelectFirstEntity()
                .GetComponent<PlayerViewComponent>()
                .Name;
            Debug.Log($"player {playerName}: player id: targetPlayerID; card key {cardComponent.Key}; ability 1: value {valueAbility_0}, ability {cardComponent.Ability_0.AbilityType}; ability 2: value {valueAbility_1}, ability {cardComponent.Ability_1.AbilityType};");
            
            #endif
        }

        private void SelectingCardToPurchase()
        {
            ref var actionData = ref _dataWorld.OneData<ActionCardData>();
            var tradePoint = actionData.TotalTrade + actionData.BonusDistrictTrade - actionData.SpendTrade;
            
            if (tradePoint == 0)
                return;

            ref var botConfigData = ref _dataWorld.OneData<BotConfigData>();
            var availableTradeRowCardEntities = _dataWorld.Select<CardComponent>()
                .With<CardTradeRowComponent>()
                .Where<CardComponent>(card => card.Price <= tradePoint)
                .GetEntities();

            var scoresCard = new List<ScoreCardToBuy>();
            foreach (var cardEntity in availableTradeRowCardEntities)
            {
                ref var cardComponent = ref cardEntity.GetComponent<CardComponent>();
                var scoreCard = CalculateOptimalCard.CalculateCardScore(cardComponent.Ability_0, botConfigData)
                    + CalculateOptimalCard.CalculateCardScore(cardComponent.Ability_1, botConfigData);
                scoreCard /= cardComponent.Price;
                scoresCard.Add(new ScoreCardToBuy { GUID = cardComponent.GUID, ScoreCard = scoreCard, Cost = cardComponent.Price});
            }

            var cardForPurchase = CalculateOptimalCard.FindOptimalPurchase(scoresCard, tradePoint);
            PurchaseCard(cardForPurchase);
        }

        private void PurchaseCard(List<string> cardForPurchase)
        {
            ref var currentPlayerID = ref _dataWorld.OneData<RoundData>().CurrentPlayerID;
            ref var actionValue = ref _dataWorld.OneData<ActionCardData>();
            var cardsParent = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CardsContainer;
            var gameUI = _dataWorld.OneData<CoreGameUIData>();

            foreach (var purchaseCardGUID in cardForPurchase)
            {
                var cardEntity = _dataWorld.Select<CardComponent>()
                    .With<CardTradeRowComponent>()
                    .Where<CardComponent>(card => card.GUID == purchaseCardGUID)
                    .SelectFirstEntity();
                
                ref var componentCard = ref cardEntity.GetComponent<CardComponent>();
                actionValue.SpendTrade += componentCard.Price;
                cardEntity.RemoveComponent<CardTradeRowComponent>();
                cardEntity.AddComponent(new CardDiscardComponent());

                componentCard.CardMono.HideCard();
                componentCard.PlayerID = currentPlayerID;
                componentCard.RectTransform.SetParent(cardsParent);
                componentCard.RectTransform.position = gameUI.BoardGameUIMono.CoreHudUIMono.DownDeck.localPosition;

                EnemyTurnViewUIAction.ShowViewEnemyCardCurrentPlayer?.Invoke(EnemyTurnActionType.PurchaseCard, componentCard.Key);
            }
            
            CardShopAction.CheckPoolShopCard?.Invoke();
        }
        
        public void Destroy()
        {
            RoundAction.StartTurnAI -= StartTurn;
        }
    }
}