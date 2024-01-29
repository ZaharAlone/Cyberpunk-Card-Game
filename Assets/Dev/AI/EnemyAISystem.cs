using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.City;
using CyberNet.Core.EnemyTurnView;
using CyberNet.Core.Player;
using CyberNet.Core.SelectFirstBase;
using CyberNet.Core.UI;
using CyberNet.Core.UI.CorePopup;
using CyberNet.Global;

namespace CyberNet.Core.AI
{
    [EcsSystem(typeof(CoreModule))]
    public class EnemyAISystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        private float _timeWaitActionBot = 0.6f;
        
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

            if (playerEntity.HasComponent<PlayerDiscardCardComponent>())
                DiscardCard();
            else
            {
                StartTurnBot();
            }
        }
        

        private void DiscardCard()
        {
            //add async, and show view discard card
            AbilityAIAction.DiscardCardSelectCard?.Invoke();
            StartTurnBot();
        }

        // Выбираем стартовую базу
        private void SelectFirstBase()
        {
            var firstBaseEntities = _dataWorld.Select<TowerComponent>()
                .With<FirstBasePlayerComponent>()
                .GetEntities();
            
            var countFirstBase = _dataWorld.Select<TowerComponent>()
                .With<FirstBasePlayerComponent>()
                .Count();

            var indexBase = Random.Range(0, countFirstBase);

            var counter = 0;
            foreach (var entityFirstBase in firstBaseEntities)
            {
                if (counter == indexBase)
                {
                    ref var towerComponent = ref entityFirstBase.GetComponent<TowerComponent>();
                    SelectFirstBaseAction.SelectBase?.Invoke(towerComponent.GUID);
                    break;
                }
                
                counter++;
            }
        }

        // Начинаем ход бота
        private void StartTurnBot()
        {
            CityAction.UpdatePresencePlayerInCity?.Invoke();
            
            /*
            var timeEntity = _dataWorld.NewEntity();
            timeEntity.AddComponent(new TimeComponent {
                Time = _timeWaitActionBot, Action = () => StartPlayingCard()
            });*/
            
            PlayCard();
            BotAIAction.EndPlayingCards += EndPlayingCards;
        }

        private void EndPlayingCards()
        {
            BotAIAction.EndPlayingCards -= EndPlayingCards;
            EnemyTurnViewUIAction.HideView?.Invoke();
            SelectTradeCard();
            
            var timeEntity = _dataWorld.NewEntity();
            timeEntity.AddComponent(new TimeComponent {
                Time = _timeWaitActionBot, Action = () => ActionPlayerButtonEvent.ActionEndTurnBot?.Invoke()
            });
        }

        private async void PlayCard()
        {
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;
            var countCard = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.PlayerID == currentPlayerID)
                .With<CardHandComponent>()
                .Count();

            if (countCard == 0)
            {
                BotAIAction.EndPlayingCards?.Invoke();
                return;
            }
            
            var selectEntityPlayCard = FindPriorityCardPlay();
                
            selectEntityPlayCard.RemoveComponent<CardHandComponent>();
            SelectionAbility(selectEntityPlayCard);   
            AbilityCardAction.UpdateValueResourcePlayedCard?.Invoke();
            var cardKey = selectEntityPlayCard.GetComponent<CardComponent>().Key;
            EnemyTurnViewUIAction.PlayingCardShowView?.Invoke(cardKey);
            await Task.Delay(350);
            PlayCard();
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

                if (cardComponent.Ability_0.AbilityType == AbilityType.Attack
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

        private void SelectionAbility(Entity entity)
        {
            ref var cardComponent = ref entity.GetComponent<CardComponent>();

            if (cardComponent.Ability_1.AbilityType == AbilityType.None)
            {
                entity.AddComponent(new CardAbilitySelectionCompletedComponent {
                    SelectAbility = SelectAbilityEnum.Ability_0
                });
                return;
            }

            var valueAbility_0 = CalculateValueCardAction.CalculateValueCardAbility?.Invoke(cardComponent.Ability_0);
            var valueAbility_1 = CalculateValueCardAction.CalculateValueCardAbility?.Invoke(cardComponent.Ability_1);

            if (valueAbility_0 > valueAbility_1)
            {
                entity.AddComponent(new CardAbilitySelectionCompletedComponent {
                    SelectAbility = SelectAbilityEnum.Ability_0
                });
            }
            else
            {
                entity.AddComponent(new CardAbilitySelectionCompletedComponent {
                    SelectAbility = SelectAbilityEnum.Ability_1
                });
            }
        }

        private void SelectTradeCard()
        {
            ref var actionData = ref _dataWorld.OneData<ActionCardData>();
            var tradePoint = actionData.TotalTrade - actionData.SpendTrade;
            
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
                    + CalculateOptimalCard.CalculateCardScore(cardComponent.Ability_1, botConfigData)
                    + CalculateOptimalCard.CalculateCardScore(cardComponent.Ability_2, botConfigData);
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
            
            foreach (var purchaseCardGUID in cardForPurchase)
            {
                var cardEntity = _dataWorld.Select<CardComponent>()
                    .With<CardTradeRowComponent>()
                    .Where<CardComponent>(card => card.GUID == purchaseCardGUID)
                    .SelectFirstEntity();
                
                ref var componentCard = ref cardEntity.GetComponent<CardComponent>();
                actionValue.SpendTrade += componentCard.Price;
                cardEntity.RemoveComponent<CardTradeRowComponent>();
                
                componentCard.PlayerID = currentPlayerID;
                componentCard.RectTransform.SetParent(cardsParent);

                cardEntity.AddComponent(new CardMoveToDiscardComponent());
            }
            
            //AnimationsMoveAtDiscardDeckAction.AnimationsMoveAtDiscardDeck?.Invoke();
            //BoardGameUIAction.UpdateStatsPlayersCurrency?.Invoke();
            CardShopAction.CheckPoolShopCard?.Invoke();
        }
    }
}