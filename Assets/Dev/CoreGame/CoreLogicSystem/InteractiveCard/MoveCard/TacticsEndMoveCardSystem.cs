using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.Battle.TacticsMode;
using CyberNet.Core.UI;

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
            
            entityCard.RemoveComponent<InteractiveMoveComponent>();
            entityCard.RemoveComponent<InteractiveSelectCardComponent>();
            entityCard.RemoveComponent<CardHandComponent>();

            
            entityCard.AddComponent(new CardSelectInTacticsScreenComponent());
            entityCard.AddComponent(new CardMoveToTacticsScreenComponent());
            
            BattleTacticsUIAction.MoveCardToTacticsScreen?.Invoke();
            CardAnimationsHandAction.AnimationsFanCardInHand?.Invoke();
        }
        
        public void Destroy()
        {
            InteractiveActionCard.EndInteractiveCard -= EndInteractiveCard;
        }
    }
}