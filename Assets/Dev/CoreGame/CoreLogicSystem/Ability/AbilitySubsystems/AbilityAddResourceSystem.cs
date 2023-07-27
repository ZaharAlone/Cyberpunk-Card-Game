using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.Ability;
using CyberNet.Core.UI;
using Object = UnityEngine.Object;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityAddResourceSystem : IRunSystem
    {
        private DataWorld _dataWorld;

        public void Run()
        {
            var entities = _dataWorld.Select<AbilityAddResourceComponent>()
                                     .Without<CardComponentAnimations>()
                                     .GetEntities();

            foreach (var entity in entities)
            {
                AddResource(entity);
            }
        }

        private void AddResource(Entity entity)
        {
            ref var abilityAddResourceComponent = ref entity.GetComponent<AbilityAddResourceComponent>();
            ref var actionData = ref _dataWorld.OneData<AbilityData>();
            var abilityVFX = _dataWorld.OneData<CardAbilityEffectData>().CardAbilityEffect;
            ref var cardComponent = ref entity.GetComponent<CardComponent>();
            
            switch (abilityAddResourceComponent.AbilityType)
            {
                case AbilityType.Attack:
                    actionData.TotalAttack += abilityAddResourceComponent.Count;
                    CreateVisualEffect(abilityVFX.AttackAbilityVFX, abilityAddResourceComponent.Count, cardComponent.Transform.position);
                    break;
                case AbilityType.Trade:
                    actionData.TotalTrade += abilityAddResourceComponent.Count;
                    CreateVisualEffect(abilityVFX.TradeAbilityVFX, abilityAddResourceComponent.Count, cardComponent.Transform.position);
                    break;
                case AbilityType.Influence:
                    actionData.TotalInfluence += abilityAddResourceComponent.Count;
                    ActionInfluence();
                    CreateVisualEffect(abilityVFX.InfluenceAbilityVFX, abilityAddResourceComponent.Count, cardComponent.Transform.position);
                    break;
            }
            
            entity.RemoveComponent<AbilityAddResourceComponent>();
            BoardGameUIAction.UpdateStatsPlayersCurrency?.Invoke();
            VFXCardInteractivAction.UpdateVFXCard?.Invoke();
            CardShopAction.SelectCardFreeToBuy?.Invoke();
        }
        
        private void ActionInfluence()
        {
            ref var actionData = ref _dataWorld.OneData<AbilityData>();
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

        private void CreateVisualEffect(CardAbilityEffectMono targetEffect, int count, Vector3 position)
        {
            var effect = Object.Instantiate(targetEffect);
            effect.transform.position = position;
            effect.SetText(count);
            effect.Init();
            Object.Destroy(effect.gameObject, 1);
        }
    }
}