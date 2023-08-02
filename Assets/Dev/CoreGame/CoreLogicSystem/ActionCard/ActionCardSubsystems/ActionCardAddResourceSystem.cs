using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.ActionCard;
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
            
            switch (abilityAddResourceComponent.AbilityType)
            {
                case AbilityType.Attack:
                    actionData.TotalAttack += abilityAddResourceComponent.Count;
                    ActionCardVisualEffect.CreateEffect(abilityVFX.attackVFX, cardComponent.Transform.position, abilityAddResourceComponent.Count);
                    break;
                case AbilityType.Trade:
                    actionData.TotalTrade += abilityAddResourceComponent.Count;
                    ActionCardVisualEffect.CreateEffect(abilityVFX.tradeVFX,cardComponent.Transform.position, abilityAddResourceComponent.Count);
                    break;
                case AbilityType.Influence:
                    actionData.TotalInfluence += abilityAddResourceComponent.Count;
                    ActionInfluence();
                    ActionCardVisualEffect.CreateEffect(abilityVFX.influenceVFX, cardComponent.Transform.position, abilityAddResourceComponent.Count);
                    break;
            }
            
            entity.RemoveComponent<ActionCardAddResourceComponent>();
            BoardGameUIAction.UpdateStatsPlayersCurrency?.Invoke();
            VFXCardInteractivAction.UpdateVFXCard?.Invoke();
            CardShopAction.SelectCardFreeToBuy?.Invoke();
        }
        
        private void ActionInfluence()
        {
            ref var actionData = ref _dataWorld.OneData<ActionCardData>();
            var deltaInfluence = actionData.TotalInfluence - actionData.SpendInfluence;
            
            if (deltaInfluence <= 0)
                return;
            
            ref var playersRound = ref _dataWorld.OneData<RoundData>().CurrentPlayer;

            if (playersRound == PlayerEnum.Player1)
            {
                ref var playerStats = ref _dataWorld.OneData<Player1StatsData>();
                playerStats.HP += deltaInfluence;
            }
            else
            {
                ref var playerStats = ref _dataWorld.OneData<Player2StatsData>();
                playerStats.HP += deltaInfluence;
            }

            actionData.SpendInfluence += deltaInfluence;
        }
    }
}