using System;
using CyberNet.Core.AbilityCard.UI;
using CyberNet.Core.BezierCurveNavigation;
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
            AbilityCardAction.CompletePlayingAbilityCard += CompletePlayingAbilityCard;
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
                    entity.AddComponent(new AbilityCardDestroyCardComponent {
                        GUIDCardDestroyList= new()
                    });
                    AbilityCardAction.DestroyCardAbility?.Invoke(guidCard);
                    break;
                case AbilityType.SwitchUnit:
                    ActionSelectCardAddComponent(abilityCardStruct, entity);
                    AbilityCardAction.SwitchEnemyUnitMap?.Invoke(guidCard);
                    break;
                case AbilityType.EnemyDiscardCard:
                    ActionSelectCardAddComponent(abilityCardStruct, entity);
                    AbilityCardAction.DiscardCard?.Invoke(guidCard);
                    break;
                case AbilityType.UnitMove:
                    ActionSelectCardAddComponent(abilityCardStruct, entity);
                    AbilityCardAction.MoveUnit?.Invoke(guidCard);
                    break;
                case AbilityType.DestroyUnit:
                    Debug.LogError("add call event Destroy enemy unit");
                    ActionSelectCardAddComponent(abilityCardStruct, entity);
                    AbilityCardAction.DestroyEnemyUnit?.Invoke(guidCard);
                    break;
                case AbilityType.AddNoiseCard:
                    ActionSelectCardAddComponent(abilityCardStruct, entity);
                    AbilityCardAction.AddNoiseCard?.Invoke(guidCard);
                    break;
                case AbilityType.SetIce:
                    ActionSelectCardAddComponent(abilityCardStruct, entity);
                    AbilityCardAction.SetIce?.Invoke(guidCard);
                    break;
                case AbilityType.PowerPoint:
                    break;
                case AbilityType.KillPoint:
                    break;
                case AbilityType.DefencePoint:
                    break;
                case AbilityType.Disorientation:
                    break;
                case AbilityType.MoveEnemyUnit:
                    break;
                case AbilityType.TradeHack:
                    break;
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
                    AbilityCardAction.CancelDestroyCard?.Invoke(cardComponent.GUID);
                    break;
                case AbilityType.SwitchUnit:
                    break;
                case AbilityType.EnemyDiscardCard:
                    AbilityCardAction.CancelDiscardCard?.Invoke();
                    break;
                case AbilityType.UnitMove:
                    AbilityCardAction.CancelMoveUnit?.Invoke(cardComponent.GUID);
                    break;
                case AbilityType.DestroyUnit:
                    break;
                case AbilityType.AddNoiseCard:
                    AbilityCardAction.CancelAddNoiseCard?.Invoke();
                    break;
                case AbilityType.SetIce:
                    break;
                case AbilityType.PowerPoint:
                    break;
                case AbilityType.KillPoint:
                    break;
                case AbilityType.DefencePoint:
                    break;
                case AbilityType.Disorientation:
                    break;
                case AbilityType.MoveEnemyUnit:
                    break;
                case AbilityType.TradeHack:
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
            actionData.BonusDistrictTrade = 0;
            
            BoardGameUIAction.UpdateStatsPlayersCurrency?.Invoke();
        }

        private void CompletePlayingAbilityCard(string guidCard)
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();
            roundData.PauseInteractive = false;

            var entityCard = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == guidCard)
                .SelectFirstEntity();

            entityCard.RemoveComponent<AbilitySelectElementComponent>();
            entityCard.RemoveComponent<SelectTargetCardAbilityComponent>();
            entityCard.RemoveComponent<CardHandComponent>();
            entityCard.RemoveComponent<InteractiveSelectCardComponent>();
            entityCard.RemoveComponent<CardComponentAnimations>();
            
            entityCard.AddComponent(new CardStartMoveToTableComponent());
            
            CardAnimationsHandAction.AnimationsFanCardInHand?.Invoke();
            AnimationsMoveBoardCardAction.AnimationsMoveBoardCard?.Invoke();   
            
            AbilityPopupUISystemAction.ClosePopup?.Invoke();
            AbilityInputButtonUIAction.HideInputUIButton?.Invoke();
            ActionPlayerButtonEvent.UpdateActionButton?.Invoke();
        }

        public void Destroy()
        {
            _dataWorld.RemoveOneData<ActionCardData>();
            
            AbilityCardAction.UpdateValueResourcePlayedCard -= CalculateValueCard;
            AbilityCardAction.ClearActionView -= ClearAction;
            AbilityCardAction.CancelAbility -= CancelAbility;
            AbilityCardAction.CompletePlayingAbilityCard -= CompletePlayingAbilityCard;
        }
    }
}