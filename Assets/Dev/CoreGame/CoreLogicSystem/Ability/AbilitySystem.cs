using CyberNet.Core.UI;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Data.Enumerators;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using UnityEngine;

namespace CyberNet.Core.Ability
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilitySystem : IPreInitSystem, IInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AbilityEvent.UpdateValueResourcePlayedCard += CalculateValueCard;
            AbilityEvent.ClearActionView += ClearAction;
        }
        
        public void Init()
        {
            _dataWorld.CreateOneData(new AbilityData());
        }
        
        //Производим расчет карт, только когда выкладываем карты на стол
        private void CalculateValueCard()
        {
            Debug.LogError("Calculate Value Card");
            ClearData();
            
            var entities = _dataWorld.Select<CardComponent>().With<CardTableComponent>().GetEntities();

            foreach (var entity in entities)
            {
                ref var cardComponent = ref entity.GetComponent<CardComponent>();
                ref var selectAbility = ref entity.GetComponent<CardTableComponent>().SelectAbility;

                if (selectAbility == SelectAbilityEnum.Ability_0)
                    AddAbilityComponent(cardComponent.Ability_0, entity);
                else
                    AddAbilityComponent(cardComponent.Ability_1, entity);

                CheckComboEffect(cardComponent, entities);
            }

            BoardGameUIAction.UpdateStatsPlayerUI?.Invoke();
        }
        
        private void CheckComboEffect(CardComponent cardComponent, EntitiesEnumerable entities)
        {
            foreach (var entity in entities)
            {
                ref var cardComponentDeck = ref entity.GetComponent<CardComponent>();
                
                if (cardComponentDeck.GUID == cardComponent.GUID)
                    continue;

                if (cardComponentDeck.Nations == cardComponent.Nations)
                {
                    AddAbilityComponent(cardComponent.Ability_2, entity);
                    break;
                }
            }
        }

        //Add component ability
        private void AddAbilityComponent(AbilityCard abilityCard, Entity entity)
        {
            Debug.LogError("Add Ability Component");
            ref var actionData = ref _dataWorld.OneData<AbilityData>();
            
            switch (abilityCard.AbilityType)
            {
                case AbilityType.Attack:
                    entity.AddComponent(new AbilityAddResourceComponent {AbilityType = abilityCard.AbilityType, Count = abilityCard.Count});
                    break;
                case AbilityType.Trade:
                    entity.AddComponent(new AbilityAddResourceComponent {AbilityType = abilityCard.AbilityType, Count = abilityCard.Count});
                    break;
                case AbilityType.Influence:
                    entity.AddComponent(new AbilityAddResourceComponent {AbilityType = abilityCard.AbilityType, Count = abilityCard.Count});
                    break;
                case AbilityType.DrawCard:
                    ActionDrawCard(abilityCard.Count);
                    break;
                case AbilityType.DiscardCardEnemy:
                    break;
                case AbilityType.DestroyCard:
                    break;
                case AbilityType.DownCyberpsychosisEnemy:
                    break;
                case AbilityType.CloneCard:
                    break;
                case AbilityType.NoiseCard:
                    break;
                case AbilityType.ThiefCard:
                    break;
                case AbilityType.DestroyTradeCard:
                    break;
                case AbilityType.DestroyEnemyBase:
                    break;
            }
        }

        private void ActionDrawCard(int value)
        {
            ref var playersRound = ref _dataWorld.OneData<RoundData>().CurrentPlayer;
            _dataWorld.RiseEvent(new EventDistributionCard { Target = playersRound, Count = value });
            
            //View Effect
        }

        private void ClearData()
        {
            ref var actionData = ref _dataWorld.OneData<AbilityData>();
            actionData.TotalAttack = 0;
            actionData.TotalTrade = 0;
            actionData.TotalInfluence = 0;
        }

        private void ClearAction()
        {
            ref var actionData = ref _dataWorld.OneData<AbilityData>();
            actionData.TotalAttack = 0;
            actionData.TotalTrade = 0;
            actionData.TotalInfluence = 0;
            actionData.SpendAttack = 0;
            actionData.SpendTrade = 0;
            actionData.SpendInfluence = 0;
            
            BoardGameUIAction.UpdateStatsPlayerUI?.Invoke();
        }
    }
}