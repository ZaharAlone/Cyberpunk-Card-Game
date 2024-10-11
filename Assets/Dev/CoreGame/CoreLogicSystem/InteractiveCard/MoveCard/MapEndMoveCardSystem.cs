using CyberNet.Core.AbilityCard;
using CyberNet.Core.AbilityCard.DiscardCard;
using CyberNet.Core.Player;
using CyberNet.Core.UI;
using CyberNet.Global.Sound;
using EcsCore;
using Input;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using DG.Tweening;

namespace CyberNet.Core.InteractiveCard
{
    [EcsSystem(typeof(CoreModule))]
    public class MapEndMoveCardSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            InteractiveActionCard.EndInteractiveCard += EndInteractiveCard;
        }

        private void EndInteractiveCard()
        {
            var playerIsDiscardCard = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .With<PlayerIsDiscardsCardComponent>()
                .Count() > 0;
            if (playerIsDiscardCard)
                return;
            
            var roundData = _dataWorld.OneData<RoundData>();
            if (roundData.PauseInteractive)
                return;
            
            var isMove = _dataWorld.Select<InteractiveMoveComponent>().Count();
            if (isMove > 0)
                EndMove();
        }

        private void EndMove()
        {
            var entity = _dataWorld.Select<CardComponent>()
                .With<InteractiveMoveComponent>()
                .SelectFirstEntity();
            
            if (entity.HasComponent<CardPlayerComponent>())
                EndMovePlayerCard(entity);
            else if (entity.HasComponent<CardTradeRowComponent>())
                EndMoveShopCard(entity);
            
            _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.BlockRaycastPanel.SetActive(false);
        }
        
        private void EndMovePlayerCard(Entity entity)
        {
            var moveComponent = entity.GetComponent<InteractiveMoveComponent>();
            var cardComponent = entity.GetComponent<CardComponent>();
            var distance = cardComponent.RectTransform.anchoredPosition.y - moveComponent.StartCardPosition.y;
            
            entity.RemoveComponent<InteractiveMoveComponent>();
            entity.RemoveComponent<InteractiveSelectCardComponent>();

            if (entity.HasComponent<CardMoveInTargetZoneComponent>())
            {
                entity.RemoveComponent<CardMoveInTargetZoneComponent>();
                entity.RemoveComponent<CardHandComponent>();

                if (entity.HasComponent<CardComponentAnimations>())
                {
                    var animationCard = entity.GetComponent<CardComponentAnimations>();
                    animationCard.Sequence.Kill();
                    entity.RemoveComponent<CardComponentAnimations>();
                }

                entity.AddComponent(new CardStartMoveToTableComponent());
                cardComponent.Canvas.sortingOrder = 2;

                AnimationsMoveBoardCardAction.AnimationsMoveBoardCard?.Invoke();
                CardAnimationsHandAction.AnimationsFanCardInHand?.Invoke();
            }
            else
            {
                entity.RemoveComponent<CardMoveInStartZoneComponent>();
                entity.RemoveComponent<CardAbilitySelectionCompletedComponent>();
                InteractiveActionCard.ReturnAllCardInHand?.Invoke();
                SoundAction.PlaySound?.Invoke(_dataWorld.OneData<SoundData>().Sound.CancelInteractiveCard);
            }
        }

        private void EndMoveShopCard(Entity entity)
        {
            var componentMove = entity.GetComponent<InteractiveMoveComponent>();
            ref var cardComponent = ref entity.GetComponent<CardComponent>();
            var roundPlayer = _dataWorld.OneData<RoundData>();

            if (entity.HasComponent<CardMoveInTargetZoneComponent>())
            {
                entity.RemoveComponent<CardMoveInTargetZoneComponent>();

                ref var actionValue = ref _dataWorld.OneData<ActionCardData>();
                var cardsParent = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CardsContainer;
                actionValue.SpendTrade += cardComponent.Price;
                entity.RemoveComponent<CardTradeRowComponent>();

                if (entity.HasComponent<CardComponentAnimations>())
                {
                    var animationCard = entity.GetComponent<CardComponentAnimations>();
                    animationCard.Sequence.Kill();
                    entity.RemoveComponent<CardComponentAnimations>();
                }

                cardComponent.PlayerID = roundPlayer.CurrentPlayerID;
                cardComponent.RectTransform.SetParent(cardsParent);
                entity.AddComponent(new CardMoveToDiscardComponent());
                entity.AddComponent(new CardPlayerComponent());
                
                VFXCardInteractiveAction.UpdateVFXCard?.Invoke();
                AnimationsMoveAtDiscardDeckAction.AnimationsMoveAtDiscardDeck?.Invoke();
                BoardGameUIAction.UpdateStatsPlayersCurrency?.Invoke();
                CardShopAction.CheckPoolShopCard?.Invoke();
                
                ActionPlayerButtonEvent.UpdateActionButton?.Invoke();
            }
            else
            {
                entity.RemoveComponent<CardMoveInStartZoneComponent>();
                cardComponent.RectTransform.anchoredPosition = componentMove.StartCardPosition;
                SoundAction.PlaySound?.Invoke(_dataWorld.OneData<SoundData>().Sound.CancelInteractiveCard);
            }

            entity.RemoveComponent<InteractiveSelectCardComponent>();
            entity.RemoveComponent<InteractiveMoveComponent>();
        }
        
        public void Destroy()
        {
            InteractiveActionCard.EndInteractiveCard -= EndInteractiveCard;
        }
    }
}