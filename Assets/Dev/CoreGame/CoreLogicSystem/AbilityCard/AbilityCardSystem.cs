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
            var entities = _dataWorld.Select<CardComponent>().With<CardTableComponent>().GetEntities();

            foreach (var entity in entities)
            {
                ref var cardComponent = ref entity.GetComponent<CardComponent>();
                ref var cardTableComponent = ref entity.GetComponent<CardTableComponent>();

                if (!cardTableComponent.CalculateBaseAbility)
                {
                    cardTableComponent.CalculateBaseAbility = true;
                    if (cardTableComponent.SelectAbility == SelectAbilityEnum.Ability_0)
                        AddAbilityComponent(cardComponent.Ability_0, entity);
                    else
                        AddAbilityComponent(cardComponent.Ability_1, entity);
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
                    AddAbilityComponent(cardComponent.Ability_2, entity);
                    ref var cardTableComponent = ref entity.GetComponent<CardTableComponent>();
                    cardTableComponent.CalculateComboAbility = true;
                    break;
                }
            }
        }

        //Add component ability
        private void AddAbilityComponent(AbilityCardContainer abilityCardStruct, Entity entity)
        {
            switch (abilityCardStruct.AbilityType)
            {
                case AbilityType.Attack:
                    entity.AddComponent(new ActionCardAddResourceComponent {
                        AbilityType = abilityCardStruct.AbilityType,
                        Count = abilityCardStruct.Count
                    });
                    AbilityCardAction.AddResource?.Invoke();
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
                    break;
                case AbilityType.EnemyDiscardCard:
                    ActionSelectCardAddComponent(abilityCardStruct, entity);
                    AbilityCardAction.DiscardCard?.Invoke();
                    break;
                case AbilityType.AddNoiseCard:
                    ActionSelectCardAddComponent(abilityCardStruct, entity);
                    AbilityCardAction.AddNoiseCard?.Invoke();
                    break;
            }
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
            actionData.TotalAttack = 0;
            actionData.TotalTrade = 0;
            actionData.SpendAttack = 0;
            actionData.SpendTrade = 0;
            
            BoardGameUIAction.UpdateStatsPlayersCurrency?.Invoke();
        }

        public void Destroy()
        {
            _dataWorld.RemoveOneData<ActionCardData>();
        }
    }
}