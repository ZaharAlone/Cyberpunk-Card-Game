using CyberNet.Core.AbilityCard;
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

        private void EndMove()
        {
            var entity = _dataWorld.Select<CardComponent>()
                .With<InteractiveMoveComponent>()
                .SelectFirstEntity();
            
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
                
                entity.AddComponent(new CardStartMoveToTableComponent());
                cardComponent.Canvas.sortingOrder = 2;

                AnimationsMoveBoardCardAction.AnimationsMoveBoardCard?.Invoke();
                CardAnimationsHandAction.AnimationsFanCardInHand?.Invoke();
            }
            else
            {
                entity.RemoveComponent<CardAbilitySelectionCompletedComponent>();
                InteractiveActionCard.ReturnAllCardInHand?.Invoke();
                SoundAction.PlaySound?.Invoke(_dataWorld.OneData<SoundData>().Sound.CancelInteractiveCard);
            }
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
                entity.AddComponent(new CardPlayerComponent());
                
                VFXCardInteractiveAction.UpdateVFXCard?.Invoke();
                AnimationsMoveAtDiscardDeckAction.AnimationsMoveAtDiscardDeck?.Invoke();
                BoardGameUIAction.UpdateStatsPlayersCurrency?.Invoke();
                CardShopAction.CheckPoolShopCard?.Invoke();
                
                ActionPlayerButtonEvent.UpdateActionButton?.Invoke();
            }
            else
            {
                var card = entity.GetComponent<CardComponent>();
                card.RectTransform.anchoredPosition = componentMove.StartCardPosition;
                SoundAction.PlaySound?.Invoke(_dataWorld.OneData<SoundData>().Sound.CancelInteractiveCard);
            }

            entity.RemoveComponent<InteractiveSelectCardComponent>();
            entity.RemoveComponent<InteractiveMoveComponent>();
        }
        
        public void Run()
        {
            var countEntityMove = _dataWorld.Select<InteractiveMoveComponent>().Count();
            if (countEntityMove > 0)
            {
                MoveCard();

                var inputData = _dataWorld.OneData<InputData>();
                if (inputData.RightClick)
                {
                    Debug.LogError("cancel select card");
                }
            }
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

        public void Destroy()
        {
            InteractiveActionCard.EndInteractiveCard -= EndInteractiveCard;
        }
    }
}