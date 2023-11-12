using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.City;
using CyberNet.Core.Player;
using CyberNet.Core.SelectFirstBase;
using CyberNet.Core.UI;

namespace CyberNet.Core.AI
{
    [EcsSystem(typeof(CoreModule))]
    public class EnemyAISystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            RoundAction.StartTurnAI += StartTurn;
        }
        
        private void StartTurn()
        {
            var roundData = _dataWorld.OneData<RoundData>();
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();

            if (playerEntity.HasComponent<PlayerNotInstallFirstBaseComponent>())
                SelectFirstBase();
            
            if (playerEntity.HasComponent<PlayerDiscardCardComponent>())
                AbilityAIAction.DiscardCardSelectCard?.Invoke();
            
            roundData.PauseInteractive = true;
            CityAction.UpdatePresencePlayerInCity?.Invoke();
            
            LogicAI();
        }

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
                else
                {
                    counter++;
                }
            }
        }

        private async void LogicAI()
        {
            Debug.LogError("Enter Start Turn Enemy");
            await Task.Delay(800);
            PlayAll();
            await Task.Delay(800);
            SelectTradeCard();
            await Task.Delay(800);
            Attack();
            await Task.Delay(800);
            ActionPlayerButtonEvent.ActionEndTurnBot?.Invoke();
        }

        private void PlayAll()
        {
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;
            var entities = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.PlayerID == currentPlayerID)
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
                entity.AddComponent(new CardAbilitySelectionCompletedComponent {
                    SelectAbility = SelectAbilityEnum.Ability_0
                });
                return;
            }

            var valueAbility_0 = CalculateValueCardInterface(cardComponent.Ability_0);
            var valueAbility_1 = CalculateValueCardInterface(cardComponent.Ability_1);

            if (valueAbility_0 > valueAbility_1)
            {
                entity.AddComponent(new CardAbilitySelectionCompletedComponent {
                    SelectAbility = SelectAbilityEnum.Ability_0
                });
                Debug.LogError("Select Ability 0");
            }
            else
            {
                Debug.LogError("Select Ability 1");
                entity.AddComponent(new CardAbilitySelectionCompletedComponent {
                    SelectAbility = SelectAbilityEnum.Ability_1
                });
            }
        }

        private int CalculateValueCardInterface(AbilityCardContainer abilityCard)
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
            
            AnimationsMoveAtDiscardDeckAction.AnimationsMoveAtDiscardDeck?.Invoke();
            BoardGameUIAction.UpdateStatsPlayersCurrency?.Invoke();
            CardShopAction.CheckPoolShopCard?.Invoke();
        }

        private void Attack()
        {
            ref var actionData = ref _dataWorld.OneData<ActionCardData>();
            var attackPoint = actionData.TotalAttack - actionData.SpendAttack;
            
            if (attackPoint == 0)
                return;
            
            if (attackPoint >= 1)
            {
                var towerFreeSlot = EnemyAIAttackSupportAction.GetTowerFreeSlotPlayerPresence.Invoke();
                if (towerFreeSlot.Count > 0)
                {
                    SpawnUnit(towerFreeSlot, attackPoint);
                }
            }
        }

        private void UpdateViewPlayer()
        {
            CityAction.UpdatePresencePlayerInCity?.Invoke();
            BoardGameUIAction.UpdateStatsPlayersCurrency?.Invoke();
        }

        private void SpawnUnit(List<BuildFreeSlotStruct> freeSlotList, int countUnit)
        {
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player => player.PlayerID == currentPlayerID)
                .SelectFirstEntity();

            ref var playerComponent = ref playerEntity.GetComponent<PlayerComponent>();
            ref var playerViewComponent = ref playerEntity.GetComponent<PlayerViewComponent>();
            ref var actionData = ref _dataWorld.OneData<ActionCardData>();
            actionData.SpendAttack += countUnit;
            playerComponent.UnitCount -= countUnit;
            
            var counter = 0;
            foreach (var freeSlot in freeSlotList)
            {
                foreach (var solidPoint in freeSlot.SolidPointsMono)
                {
                    var unit = new InitUnitStruct 
                    {
                        KeyUnit = playerViewComponent.KeyCityVisual,
                        SquadZone = solidPoint,
                        PlayerControl = PlayerControlEnum.Player,
                        TargetPlayerID = currentPlayerID
                    };
                    CityAction.InitUnit?.Invoke(unit);

                    counter++;
                    
                    if (counter >= countUnit)
                        break;
                }
                
                if (counter >= countUnit)
                    break;
            }

            UpdateViewPlayer();
        }
    }
}