using CyberNet.Core.UI;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Data.Enumerators;
using ModulesFramework.Systems;
using UnityEngine;

namespace CyberNet.Core.ActionCard
{
    [EcsSystem(typeof(CoreModule))]
    public class ActionCardSystem : IPreInitSystem, IInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            ActionCardEvent.UpdateValueResourcePlayedCard += CalculateValueCard;
            ActionCardEvent.ClearActionView += ClearAction;
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
        private void AddAbilityComponent(AbilityCard abilityCard, Entity entity)
        {
            switch (abilityCard.AbilityType)
            {
                case AbilityType.Attack:
                    entity.AddComponent(new ActionCardAddResourceComponent {AbilityType = abilityCard.AbilityType, Count = abilityCard.Count});
                    break;
                case AbilityType.Trade:
                    entity.AddComponent(new ActionCardAddResourceComponent {AbilityType = abilityCard.AbilityType, Count = abilityCard.Count});
                    break;
                case AbilityType.DrawCard:
                    ActionDrawCard(abilityCard.Count);
                    break;
                case AbilityType.DestroyCard:
                    ActionSelectCardAddComponent(abilityCard, entity);
                    break;
            }
        }

        private void ActionSelectCardAddComponent(AbilityCard abilityCard, Entity entity)
        {
            entity.AddComponent(new ActionSelectCardComponent {
                AbilityCard = abilityCard
            });
            ActionSelectCardAction.OpenSelectAbilityCard?.Invoke();
        }

        private void ActionDrawCard(int value)
        {
            ref var targetPlayerID = ref _dataWorld.OneData<RoundData>().CurrentPlayerID;
            _dataWorld.RiseEvent(new EventDistributionCard { TargetPlayerID = targetPlayerID, Count = value });
            
            //View Effect
        }

        private void ClearAction()
        {
            ref var actionData = ref _dataWorld.OneData<ActionCardData>();
            actionData.TotalAttack = 0;
            actionData.TotalTrade = 0;
            actionData.TotalInfluence = 0;
            actionData.SpendAttack = 0;
            actionData.SpendTrade = 0;
            actionData.SpendInfluence = 0;
            
            BoardGameUIAction.UpdateStatsPlayersCurrency?.Invoke();
        }

        public void Destroy()
        {
            _dataWorld.RemoveOneData<ActionCardData>();
        }
    }
}