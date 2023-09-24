using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.UI;
using Object = UnityEngine.Object;

namespace CyberNet.Core.AbilityCard
{
    [EcsSystem(typeof(CoreModule))]
    public class ActionCardDiscardCardSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            ActionCardEvent.CheckDiscardCard += CheckDiscardCard;
        }

        //NOT WORK RUN
        public void Run()
        {
            var entities = _dataWorld.Select<ActionCardDiscardCardVisualEffect>()
                .Without<CardComponentAnimations>()
                .GetEntities();

            foreach (var entity in entities)
            {
                DiscardCardEffect(entity);
            }
        }

        private void DiscardCardEffect(Entity entity)
        {
            ref var playerTargetDiscard = ref entity.GetComponent<ActionCardDiscardCardVisualEffect>().TargetDiscardCardPlayerID;
            var abilityVFX = _dataWorld.OneData<ActionCardConfigData>().ActionCardConfig;
            ref var cardComponent = ref entity.GetComponent<CardComponent>();
            ref var cardsContainer = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CardsContainer;
            
            ActionCardVisualEffect.CreateEffect(abilityVFX.discardActionCardVFX, cardComponent.RectTransform.position, cardsContainer);

            ref var gameUI = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;
            ref var currentPlayerID = ref _dataWorld.OneData<RoundData>().CurrentPlayerID;

            //TODO: старый код
            /*
            if (currentPlayerID == playerTargetDiscard)
            {
                Object.Instantiate(abilityVFX.DiscardCardUIEffect, gameUI.CoreHudUIMono.PlayerDownView.FrameEffectCard);
            }
            else
            {
                Object.Instantiate(abilityVFX.DiscardCardUIEffect, gameUI.CoreHudUIMono.PlayerUpView.FrameEffectCard);
            }
            */
            entity.RemoveComponent<ActionCardDiscardCardVisualEffect>();

            var newEntity = _dataWorld.NewEntity();
            newEntity.AddComponent(new ActionCardDiscardCardComponent {
                TargetDiscardCardID = playerTargetDiscard
            });
        }
        
        private void CheckDiscardCard()
        {
            var entities = _dataWorld.Select<ActionCardDiscardCardComponent>()
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
            ref var playerTargetDiscard = ref entity.GetComponent<ActionCardDiscardCardComponent>().TargetDiscardCardID;
            ref var gameUI = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;
            ref var currentPlayerID = ref _dataWorld.OneData<RoundData>().CurrentPlayerID;

            /*
            if (currentPlayerID == playerTargetDiscard)
            {
                Object.Destroy(gameUI.CoreHudUIMono.PlayerDownView.FrameEffectCard.GetChild(0).gameObject);
            }
            else
            {
                //TODO: старый код
                //Object.Destroy(gameUI.CoreHudUIMono.PlayerUpView.FrameEffectCard.GetChild(0).gameObject);
            }*/
        }

        private void DiscardSelectCard()
        {
            //Discard select card in ui
            
            ActionCardEvent.CheckDiscardCard?.Invoke();
        }
    }
}