using System;
using CyberNet.Core.InteractiveCard;
using CyberNet.Core.UI;
using CyberNet.Global;
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
            AbilityCardAction.CancelAbility += CancelAbility;
        }
        
        public void Init()
        {
            _dataWorld.CreateOneData(new ActionCardData());
        }
        
        //Производим расчет карт, только когда выкладываем карты на стол
        private void CalculateValueCard()
        {
            var cardInTableEntities = _dataWorld.Select<CardComponent>()
                .With<CardAbilitySelectionCompletedComponent>()
                .GetEntities();

            foreach (var cardInTableEntity in cardInTableEntities)
            {
                ref var cardComponent = ref cardInTableEntity
                    .GetComponent<CardComponent>();
                ref var cardSelectAbilityComponent = ref cardInTableEntity
                    .GetComponent<CardAbilitySelectionCompletedComponent>();

                if (!cardSelectAbilityComponent.CalculateBaseAbility)
                {
                    cardSelectAbilityComponent.CalculateBaseAbility = true;
                    if (cardSelectAbilityComponent.SelectAbility == SelectAbilityEnum.Ability_0)
                        AddAbilityComponent(cardComponent.GUID, cardComponent.Ability_0, cardInTableEntity);
                    else
                        AddAbilityComponent(cardComponent.GUID, cardComponent.Ability_1, cardInTableEntity);
                }
                
                CheckComboEffect(cardInTableEntity);
            }
        }
        
        private void CheckComboEffect(Entity currentCardEntity)
        {
            ref var currentCardComponent = ref currentCardEntity
                .GetComponent<CardComponent>();

            var cardInTableEntities = _dataWorld.Select<CardComponent>()
                .With<CardAbilitySelectionCompletedComponent>()
                .GetEntities();
            
            foreach (var cardInTableEntity in cardInTableEntities)
            {
                ref var cardComponent = ref cardInTableEntity.GetComponent<CardComponent>();
                ref var cardSelectAbilityComponent = ref cardInTableEntity
                    .GetComponent<CardAbilitySelectionCompletedComponent>();
                
                if (cardComponent.GUID == currentCardComponent.GUID || cardComponent.Nations == CardNations.Neutral)
                    continue;
                
                if (cardComponent.Nations == currentCardComponent.Nations && !cardSelectAbilityComponent.CalculateComboAbility)
                {
                    AddAbilityComponent(cardComponent.GUID, cardComponent.Ability_2, cardInTableEntity);
                    cardSelectAbilityComponent.CalculateComboAbility = true;
                }
            }
        }

        //Add component ability
        private void AddAbilityComponent(string guidCard, AbilityCardContainer abilityCardStruct, Entity entity)
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();
            
            if (abilityCardStruct.AbilityType != AbilityType.Trade && roundData.playerOrAI == PlayerOrAI.Player)
            {
                _dataWorld.OneData<RoundData>().PauseInteractive = true;
                AbilityCardAction.ShiftUpCard?.Invoke(guidCard);
            }
            
            switch (abilityCardStruct.AbilityType)
            {
                case AbilityType.AddUnit:
                    ActionSelectCardAddComponent(abilityCardStruct, entity);
                    entity.AddComponent(new AbilityCardAddUnitComponent {
                        ListTowerAddUnit = new()
                    });
                    AbilityCardAction.AbilityAddUnitMap?.Invoke(guidCard);
                    break;
                case AbilityType.Trade:
                    entity.AddComponent(new AbilityCardAddResourceComponent {
                        AbilityType = abilityCardStruct.AbilityType,
                        Count = abilityCardStruct.Count
                    });
                    AbilityCardAction.AddTradePoint?.Invoke();
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
                    AbilityCardAction.DiscardCard?.Invoke(guidCard);
                    break;
                case AbilityType.UnitMove:
                    ActionSelectCardAddComponent(abilityCardStruct, entity);
                    AbilityCardAction.MoveUnit?.Invoke(guidCard);
                    break;
                case AbilityType.DestroyNeutralUnit:
                    Debug.LogError("add call event Destroy neutral unit");
                    ActionSelectCardAddComponent(abilityCardStruct, entity);
                    AbilityCardAction.DestroyNeutralUnit?.Invoke(guidCard);
                    break;
                case AbilityType.DestroyUnit:
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
                case AbilityType.SwitchEnemyUnit:
                    ActionSelectCardAddComponent(abilityCardStruct, entity);
                    AbilityCardAction.SwitchEnemyUnitMap?.Invoke(guidCard);
                    break;
                case AbilityType.SwitchNeutralUnit:
                    ActionSelectCardAddComponent(abilityCardStruct, entity);
                    AbilityCardAction.SwitchNeutralUnitMap?.Invoke(guidCard);
                    break;
                case AbilityType.HeadShot:
                    ActionSelectCardAddComponent(abilityCardStruct, entity);
                    AbilityCardAction.HeadShot?.Invoke(guidCard);
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
                case AbilityType.AddUnit:
                    AbilityCardAction.CancelAddUnitMap?.Invoke(cardComponent.GUID);
                    break;
                case AbilityType.DestroyCard:
                    break;
                case AbilityType.CloneCard:
                    break;
                case AbilityType.DestroyTradeCard:
                    break;
                case AbilityType.SwitchNeutralUnit:
                    break;
                case AbilityType.SwitchEnemyUnit:
                    break;
                case AbilityType.DestroyNeutralUnit:
                    break;
                case AbilityType.DestroyUnit:
                    break;
                case AbilityType.EnemyDiscardCard:
                    break;
                case AbilityType.UnitMove:
                    AbilityCardAction.CancelMoveUnit?.Invoke(cardComponent.GUID);
                    break;
                case AbilityType.SetIce:
                    break;
                case AbilityType.DestroyIce:
                    break;
            }
            
            entity.RemoveComponent<SelectTargetCardAbilityComponent>();
            entity.RemoveComponent<CardAbilitySelectionCompletedComponent>();
            entity.RemoveComponent<AbilitySelectElementComponent>();

            _dataWorld.OneData<RoundData>().PauseInteractive = false;
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
            AbilityCardAction.CancelAbility -= CancelAbility;
        }
    }
}