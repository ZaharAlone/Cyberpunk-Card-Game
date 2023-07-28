using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.UI;
using Object = UnityEngine.Object;

namespace CyberNet.Core.Ability
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityDiscardCardSystem : IPreInitSystem, IRunSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AbilityEvent.CheckDiscardCard += CheckDiscardCard;
        }

        public void Run()
        {
            var entities = _dataWorld.Select<AbilityDiscardCardVisualEffect>()
                .Without<CardComponentAnimations>()
                .GetEntities();

            foreach (var entity in entities)
            {
                DiscardCardEffect(entity);
            }
        }

        private void DiscardCardEffect(Entity entity)
        {
            ref var playerTargetDiscard = ref entity.GetComponent<AbilityDiscardCardVisualEffect>().TargetDiscardCard;
            var abilityVFX = _dataWorld.OneData<CardAbilityEffectData>().CardAbilityEffect;
            ref var cardComponent = ref entity.GetComponent<CardComponent>();

            AbilityVisualEffect.CreateEffect(abilityVFX.DiscardCardVFX, cardComponent.Transform.position);

            ref var gameUI = ref _dataWorld.OneData<UIData>().UIMono;
            ref var playerView = ref _dataWorld.OneData<ViewPlayerData>().PlayerView;

            if (playerView == playerTargetDiscard)
            {
                Object.Instantiate(abilityVFX.DiscardCardUIEffect, gameUI.CoreHudUIMono.PlayerDownView.FrameEffectCard);
            }
            else
            {
                Object.Instantiate(abilityVFX.DiscardCardUIEffect, gameUI.CoreHudUIMono.PlayerUpView.FrameEffectCard);
            }
            
            entity.RemoveComponent<AbilityDiscardCardVisualEffect>();

            var newEntity = _dataWorld.NewEntity();
            newEntity.AddComponent(new AbilityDiscardCardComponent {
                TargetDiscardCard = playerTargetDiscard
            });
        }
        
        private void CheckDiscardCard()
        {
            var entities = _dataWorld.Select<AbilityDiscardCardComponent>()
                .Without<CardComponentAnimations>()
                .GetEntities();

            foreach (var entity in entities)
            {
                DiscardCard(entity);
                break;
            }
        }

        private void DiscardCard(Entity entity)
        {
            Debug.LogError("DiscardCard");
            //TODO: Show ui select discard card;
            ref var playerTargetDiscard = ref entity.GetComponent<AbilityDiscardCardComponent>().TargetDiscardCard;
            ref var gameUI = ref _dataWorld.OneData<UIData>().UIMono;
            ref var viewPlayer = ref _dataWorld.OneData<ViewPlayerData>().PlayerView;

            if (viewPlayer == playerTargetDiscard)
            {
                Object.Destroy(gameUI.CoreHudUIMono.PlayerDownView.FrameEffectCard.GetChild(0).gameObject);
            }
            else
            {
                Object.Destroy(gameUI.CoreHudUIMono.PlayerUpView.FrameEffectCard.GetChild(0).gameObject);
            }
        }

        private void DiscardSelectCard()
        {
            //Discard select card in ui
            
            AbilityEvent.CheckDiscardCard?.Invoke();
        }
    }
}