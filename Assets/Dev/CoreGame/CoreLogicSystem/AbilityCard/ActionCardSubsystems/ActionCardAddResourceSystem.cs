using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.UI;
using Object = UnityEngine.Object;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class ActionCardAddResourceSystem : IRunSystem
    {
        private DataWorld _dataWorld;

        public void Run()
        {
            var entities = _dataWorld.Select<ActionCardAddResourceComponent>()
                                     .Without<CardComponentAnimations>()
                                     .GetEntities();

            foreach (var entity in entities)
            {
                AddResource(entity);
            }
        }

        private void AddResource(Entity entity)
        {
            ref var abilityAddResourceComponent = ref entity.GetComponent<ActionCardAddResourceComponent>();
            ref var actionData = ref _dataWorld.OneData<ActionCardData>();
            var abilityVFX = _dataWorld.OneData<ActionCardConfigData>().ActionCardConfig;
            ref var cardComponent = ref entity.GetComponent<CardComponent>();
            ref var cardsContainer = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CardsContainer;
            
            switch (abilityAddResourceComponent.AbilityType)
            {
                case AbilityType.Attack:
                    actionData.TotalAttack += abilityAddResourceComponent.Count;
                    ActionCardVisualEffect.CreateEffect(abilityVFX.attackVFX, cardComponent.RectTransform.position, cardsContainer, abilityAddResourceComponent.Count);
                    break;
                case AbilityType.Trade:
                    actionData.TotalTrade += abilityAddResourceComponent.Count;
                    ActionCardVisualEffect.CreateEffect(abilityVFX.tradeVFX,cardComponent.RectTransform.position, cardsContainer, abilityAddResourceComponent.Count);
                    break;
            }
            
            entity.RemoveComponent<ActionCardAddResourceComponent>();
            BoardGameUIAction.UpdateStatsPlayersCurrency?.Invoke();
        }
    }
}