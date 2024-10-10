using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.Battle.TacticsMode;
using CyberNet.Core.Battle.TacticsMode.InteractiveCard;
using CyberNet.Core.UI;
using DG.Tweening;

namespace CyberNet.Core.InteractiveCard
{
    [EcsSystem(typeof(CoreModule))]
    public class TacticsEndMoveCardSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            InteractiveActionCard.EndInteractiveCard += EndInteractiveCard;
        }

        private void EndInteractiveCard()
        {
            var isOffTacticsScreen = _dataWorld.Select<OpenBattleTacticsUIComponent>().Count() == 0;
            if (isOffTacticsScreen)
                return;
            
            var entityCard = _dataWorld.Select<CardComponent>()
                .With<InteractiveMoveComponent>()
                .SelectFirstEntity();
            
            if (entityCard.HasComponent<CardComponentAnimations>())
            {
                var animationCard = entityCard.GetComponent<CardComponentAnimations>();
                animationCard.Sequence.Kill();
                entityCard.RemoveComponent<CardComponentAnimations>();
            }
            
            entityCard.RemoveComponent<InteractiveMoveComponent>();
            entityCard.RemoveComponent<InteractiveSelectCardComponent>();

            if (entityCard.HasComponent<CardSelectInTacticsScreenComponent>())
            {
                Debug.LogError("Remove card tactics screen component");
                entityCard.AddComponent(new CardHandComponent());
                entityCard.RemoveComponent<CardSelectInTacticsScreenComponent>();
            }
            else if (entityCard.HasComponent<CardHandComponent>())
            {
                entityCard.RemoveComponent<CardHandComponent>();
                entityCard.AddComponent(new CardSelectInTacticsScreenComponent());
                entityCard.AddComponent(new CardMoveToTacticsScreenComponent());
                
                BattleTacticsUIAction.MoveCardToTacticsScreen?.Invoke();
            }
            
            CardAnimationsHandAction.AnimationsFanCardInHand?.Invoke();
        }
        
        public void Destroy()
        {
            InteractiveActionCard.EndInteractiveCard -= EndInteractiveCard;
        }
    }
}