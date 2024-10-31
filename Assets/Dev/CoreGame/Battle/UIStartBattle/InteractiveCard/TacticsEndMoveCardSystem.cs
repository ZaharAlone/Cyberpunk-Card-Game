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
            BattleTacticsUIAction.EndMoveCardTactics += EndInteractiveCard;
        }

        private void EndInteractiveCard()
        {
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

            if (entityCard.HasComponent<CardMoveInStartZoneComponent>())
            {
                entityCard.RemoveComponent<CardMoveInStartZoneComponent>();
                if (entityCard.HasComponent<CardSelectInTacticsScreenComponent>())
                {
                    entityCard.RemoveComponent<CardSelectInTacticsScreenComponent>();
                    entityCard.AddComponent(new CardHandComponent());
                }
            }
            else if (entityCard.HasComponent<CardMoveInTargetZoneComponent>())
            {
                entityCard.RemoveComponent<CardMoveInTargetZoneComponent>();
                
                var cardComponent = entityCard.GetComponent<CardComponent>();
                cardComponent.CardMono.CardFaceMono.VFXDisable();
                
                if (entityCard.HasComponent<CardHandComponent>())
                {
                    entityCard.RemoveComponent<CardHandComponent>();
                    entityCard.AddComponent(new CardSelectInTacticsScreenComponent());
                }

                entityCard.AddComponent(new CardMoveToTacticsScreenComponent());
                
                BattleTacticsUIAction.MoveCardToTacticsScreen?.Invoke();
            }
            
            CardAnimationsHandAction.AnimationsFanCardInTacticsScreen?.Invoke();
            _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.BlockRaycastPanel.SetActive(false);
            
            BattleTacticsUIAction.UpdateCardAndTactics?.Invoke();
        }
        
        public void Destroy()
        {
            InteractiveActionCard.EndInteractiveCard -= EndInteractiveCard;
        }
    }
}