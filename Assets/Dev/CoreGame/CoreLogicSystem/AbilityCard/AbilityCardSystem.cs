using System;
using CyberNet.Core.InteractiveCard;
using CyberNet.Core.UI;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Data.Enumerators;
using ModulesFramework.Systems;
using UnityEngine;

namespace CyberNet.Core.AbilityCard
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityCardSystem : IPreInitSystem, IInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AbilityCardAction.UpdateValueResourcePlayedCard += CalculateValueCard;
            AbilityCardAction.ClearActionView += ClearAction;
        }
        
        public void Init()
        {
            _dataWorld.CreateOneData(new ActionCardData());
        }
        
        //Производим расчет карт, только когда выкладываем карты на стол
        private void CalculateValueCard()
        {
            var entities = _dataWorld.Select<CardComponent>().With<CardAbilitySelectionCompletedComponent>().GetEntities();

            foreach (var entity in entities)
            {
                ref var cardComponent = ref entity.GetComponent<CardComponent>();
                ref var cardTableComponent = ref entity.GetComponent<CardAbilitySelectionCompletedComponent>();

                if (!cardTableComponent.CalculateBaseAbility)
                {
                    cardTableComponent.CalculateBaseAbility = true;
                    if (cardTableComponent.SelectAbility == SelectAbilityEnum.Ability_0)
                        AddAbilityComponent(cardComponent.GUID, cardComponent.Ability_0, entity);
                    else
                        AddAbilityComponent(cardComponent.GUID, cardComponent.Ability_1, entity);
                }

                if (!cardTableComponent.CalculateComboAbility)
                {
                    CheckComboEffect(cardComponent, entities);   
                }
            }
        }
        
        private void CheckComboEffect(CardComponent cardComponent, EntitiesEnumerable entities)
        {
            //TODO: дописать скорее всего сейчас нифига не работает так как нет проверки условий
            foreach (var entity in entities)
            {
                ref var cardComponentDeck = ref entity.GetComponent<CardComponent>();
                
                if (cardComponentDeck.GUID == cardComponent.GUID || cardComponentDeck.Nations == CardNations.Neutral)
                    continue;
                
                if (cardComponentDeck.Nations == cardComponent.Nations)
                {
                    AddAbilityComponent(cardComponent.GUID, cardComponent.Ability_2, entity);
                    ref var cardTableComponent = ref entity.GetComponent<CardAbilitySelectionCompletedComponent>();
                    cardTableComponent.CalculateComboAbility = true;
                    break;
                }
            }
        }

        //Add component ability
        private void AddAbilityComponent(string guidCard, AbilityCardContainer abilityCardStruct, Entity entity)
        {
            switch (abilityCardStruct.AbilityType)
            {
                case AbilityType.Attack:
                    ActionSelectCardAddComponent(abilityCardStruct, entity);
                    entity.AddComponent(new AbilityCardAddUnitComponent {
                        ListTowerAddUnit = new()
                    });
                    AbilityCardAction.AbilityAddUnitMap?.Invoke(guidCard);
                    break;
                case AbilityType.Trade:
                    entity.AddComponent(new ActionCardAddResourceComponent {
                        AbilityType = abilityCardStruct.AbilityType,
                        Count = abilityCardStruct.Count
                    });
                    AbilityCardAction.AddResource?.Invoke();
                    break;
                case AbilityType.DrawCard:
                    ActionDrawCard(abilityCardStruct.Count);
                    break;
                case AbilityType.DestroyCard:
                    ActionSelectCardAddComponent(abilityCardStruct, entity);
                    AbilityCardAction.DestroyCardAbility?.Invoke(guidCard);
                    break;
                case AbilityType.EnemyDiscardCard:
                    ActionSelectCardAddComponent(abilityCardStruct, entity);
                    AbilityCardAction.DiscardCard?.Invoke();
                    break;
                case AbilityType.SquadMove:
                    ActionSelectCardAddComponent(abilityCardStruct, entity);
                    AbilityCardAction.MoveUnit?.Invoke(guidCard);
                    break;
                case AbilityType.DestroyNeutralSquad:
                    Debug.LogError("add call event Destroy neutral unit");
                    ActionSelectCardAddComponent(abilityCardStruct, entity);
                    AbilityCardAction.DestroyNeutralUnit?.Invoke(guidCard);
                    break;
                case AbilityType.DestroySquad:
                    Debug.LogError("add call event Destroy enemy unit");
                    ActionSelectCardAddComponent(abilityCardStruct, entity);
                    AbilityCardAction.DestroyEnemyUnit?.Invoke(guidCard);
                    break;
                case AbilityType.SetIce:
                    ActionSelectCardAddComponent(abilityCardStruct, entity);
                    AbilityCardAction.SetIce?.Invoke(guidCard);
                    break;
                case AbilityType.DestroyIce:
                    ActionSelectCardAddComponent(abilityCardStruct, entity);
                    AbilityCardAction.DestroyIce?.Invoke(guidCard);
                    break;
                case AbilityType.SwitchEnemySquad:
                    ActionSelectCardAddComponent(abilityCardStruct, entity);
                    AbilityCardAction.SwitchEnemyUnitMap?.Invoke(guidCard);
                    break;
                case AbilityType.SwitchNeutralSquad:
                    ActionSelectCardAddComponent(abilityCardStruct, entity);
                    AbilityCardAction.SwitchNeutralUnitMap?.Invoke(guidCard);
                    break;

                /*
                case AbilityType.AddNoiseCard:
                    ActionSelectCardAddComponent(abilityCardStruct, entity);
                    AbilityCardAction.AddNoiseCard?.Invoke();
                    break;*/
            }
        }

        private void CancelAbility()
        {
            var entity = _dataWorld.Select<CardAbilitySelectionCompletedComponent>().SelectFirstEntity();
            var cardSelectAbilityComponent = entity.GetComponent<CardAbilitySelectionCompletedComponent>();

            var cardComponent = entity.GetComponent<CardComponent>();
            var selectAbility = AbilityType.None;

            if (cardSelectAbilityComponent.SelectAbility == SelectAbilityEnum.Ability_0)
                selectAbility = cardComponent.Ability_0.AbilityType;
            else
                selectAbility = cardComponent.Ability_1.AbilityType;

            switch (selectAbility)
            {
                case AbilityType.Attack:
                    break;
                case AbilityType.DestroyCard:
                    break;
                case AbilityType.CloneCard:
                    break;
                case AbilityType.DestroyTradeCard:
                    break;
                case AbilityType.SwitchNeutralSquad:
                    break;
                case AbilityType.SwitchEnemySquad:
                    break;
                case AbilityType.DestroyNeutralSquad:
                    break;
                case AbilityType.DestroySquad:
                    break;
                case AbilityType.EnemyDiscardCard:
                    break;
                case AbilityType.SquadMove:
                    break;
                case AbilityType.SetIce:
                    break;
                case AbilityType.DestroyIce:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            
            entity.RemoveComponent<SelectTargetCardAbilityComponent>();
            entity.RemoveComponent<CardAbilitySelectionCompletedComponent>();
        }
        
        private void ActionSelectCardAddComponent(AbilityCardContainer abilityCardStruct, Entity entity)
        {
            entity.AddComponent(new AbilitySelectElementComponent {
                AbilityCard = abilityCardStruct
            });
        }
        
        private void ActionDrawCard(int value)
        {
            ref var targetPlayerID = ref _dataWorld.OneData<RoundData>().CurrentPlayerID;
            _dataWorld.RiseEvent(new EventDistributionCard { TargetPlayerID = targetPlayerID, Count = value });
        }

        private void ClearAction()
        {
            ref var actionData = ref _dataWorld.OneData<ActionCardData>();
            actionData.TotalTrade = 0;
            actionData.SpendTrade = 0;
            
            BoardGameUIAction.UpdateStatsPlayersCurrency?.Invoke();
        }

        public void Destroy()
        {
            _dataWorld.RemoveOneData<ActionCardData>();
            
            AbilityCardAction.UpdateValueResourcePlayedCard -= CalculateValueCard;
            AbilityCardAction.ClearActionView -= ClearAction;
        }
    }
}