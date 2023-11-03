using CyberNet.Core.AbilityCard;
using CyberNet.Core.UI;
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
    public class InteractiveMoveCardSystem : IPreInitSystem, IRunSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            InteractiveActionCard.EndInteractiveCard += EndInteractiveCard;
        }

        private void EndInteractiveCard()
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();
            if (roundData.PauseInteractive)
                return;
            
            var isMove = _dataWorld.Select<InteractiveMoveComponent>().Count();
            if (isMove > 0)
                EndMove();
        }

        public void Run()
        {
            var countEntityMove = _dataWorld.Select<InteractiveMoveComponent>().Count();
            if (countEntityMove > 0)
                MoveCard();
        }

        private void MoveCard()
        {
            var entities = _dataWorld.Select<CardComponent>().With<InteractiveMoveComponent>().GetEntities();
            ref var inputData = ref _dataWorld.OneData<InputData>();

            foreach (var entity in entities)
            {
                ref var componentMove = ref entity.GetComponent<InteractiveMoveComponent>();
                ref var componentCard = ref entity.GetComponent<CardComponent>();

                var deltaMove = inputData.MousePosition - componentMove.StartMousePositions;
                componentCard.RectTransform.anchoredPosition += new Vector2(deltaMove.x, deltaMove.y);
                componentMove.StartMousePositions = inputData.MousePosition;
            }
        }

        private void EndMove()
        {
            var entity = _dataWorld.Select<CardComponent>().With<InteractiveMoveComponent>().SelectFirstEntity();
            
            if (entity.HasComponent<CardPlayerComponent>())
                EndMovePlayerCard(entity);
            else if (entity.HasComponent<CardTradeRowComponent>())
                EndMoveShopCard(entity);
        }

        private void EndMovePlayerCard(Entity entity)
        {
            var moveComponent = entity.GetComponent<InteractiveMoveComponent>();
            var cardComponent = entity.GetComponent<CardComponent>();
            var distance = cardComponent.RectTransform.anchoredPosition.y - moveComponent.StartCardPosition.y;
            ref var currentPlayerID = ref _dataWorld.OneData<RoundData>().CurrentPlayerID;
            
            entity.RemoveComponent<InteractiveMoveComponent>();
            entity.RemoveComponent<InteractiveSelectCardComponent>();

            if (distance > 140)
            {
                entity.RemoveComponent<CardHandComponent>();

                if (entity.HasComponent<CardComponentAnimations>())
                {
                    var animationCard = entity.GetComponent<CardComponentAnimations>();
                    animationCard.Sequence.Kill();
                    entity.RemoveComponent<CardComponentAnimations>();
                }

                entity.AddComponent(new CardSelectAbilityComponent());
                cardComponent.Canvas.sortingOrder = 2;

                AnimationsMoveBoardCardAction.AnimationsMoveBoardCard?.Invoke();
                _dataWorld.RiseEvent(new EventCardAnimationsHand { TargetPlayerID = currentPlayerID });
            }
            else
            {
                InteractiveActionCard.ReturnAllCardInHand?.Invoke();
            }
            
            ActionPlayerButtonEvent.UpdateActionButton?.Invoke();
        }

        private void EndMoveShopCard(Entity entity)
        {
            var componentMove = entity.GetComponent<InteractiveMoveComponent>();
            ref var componentCard = ref entity.GetComponent<CardComponent>();
            var distance = componentCard.RectTransform.anchoredPosition.y - componentMove.StartCardPosition.y;
            var roundPlayer = _dataWorld.OneData<RoundData>();

            if (Mathf.Abs(distance) > 175)
            {
                ref var actionValue = ref _dataWorld.OneData<ActionCardData>();
                var cardsParent = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CardsContainer;
                actionValue.SpendTrade += componentCard.Price;
                entity.RemoveComponent<CardTradeRowComponent>();

                if (entity.HasComponent<CardComponentAnimations>())
                {
                    var animationCard = entity.GetComponent<CardComponentAnimations>();
                    animationCard.Sequence.Kill();
                    entity.RemoveComponent<CardComponentAnimations>();
                }

                componentCard.PlayerID = roundPlayer.CurrentPlayerID;
                componentCard.RectTransform.SetParent(cardsParent);
                entity.AddComponent(new CardMoveToDiscardComponent());
                
                VFXCardInteractivAction.UpdateVFXCard?.Invoke();
                AnimationsMoveAtDiscardDeckAction.AnimationsMoveAtDiscardDeck?.Invoke();
                BoardGameUIAction.UpdateStatsPlayersCurrency?.Invoke();
                CardShopAction.CheckPoolShopCard?.Invoke();
            }
            else
            {
                var card = entity.GetComponent<CardComponent>();
                card.RectTransform.anchoredPosition = componentMove.StartCardPosition;
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