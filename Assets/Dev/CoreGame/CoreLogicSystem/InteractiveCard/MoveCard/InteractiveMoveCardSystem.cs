using CyberNet.Core.Ability;
using CyberNet.Core.UI;
using EcsCore;
using Input;
using ModulesFrameworkUnity;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using DG.Tweening;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class InteractiveMoveCardSystem : IInitSystem, IRunSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            InteractiveActionCard.StartInteractiveCard += DownClickCard;
            InteractiveActionCard.EndInteractiveCard += UpClickCard;
        }

        private void DownClickCard(string guid)
        {
            var entity = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == guid)
                .SelectFirstEntity();

            ref var component = ref entity.GetComponent<CardComponent>();
            var round = _dataWorld.OneData<RoundData>();
            if (round.CurrentPlayer != component.Player && component.Player != PlayerEnum.None)
                return;

            if (entity.HasComponent<CardHandComponent>() || entity.HasComponent<CardFreeToBuyComponent>())
            {
                ref var inputData = ref _dataWorld.OneData<InputData>();

                entity.AddComponent(new InteractiveMoveComponent
                {
                    StartCardPosition = component.Transform.position,
                    StartCardRotation = component.Transform.rotation,
                    StartMousePositions = inputData.MousePosition
                });
            }
        }

        private void UpClickCard()
        {
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
                componentCard.Transform.position += new Vector3(deltaMove.x, deltaMove.y, 0);
                componentMove.StartMousePositions = inputData.MousePosition;
            }
        }

        private void EndMove()
        {
            var entity = _dataWorld.Select<CardComponent>().With<InteractiveMoveComponent>().SelectFirstEntity();
            ref var cardComponent = ref entity.GetComponent<CardComponent>();

            if (cardComponent.Player != PlayerEnum.None)
                EndMovePlayerCard(entity);
            else if (entity.HasComponent<CardTradeRowComponent>())
                EndMoveShopCard(entity);
        }

        private void EndMovePlayerCard(Entity entity)
        {
            var moveComponent = entity.GetComponent<InteractiveMoveComponent>();
            var cardComponent = entity.GetComponent<CardComponent>();
            var distance = cardComponent.Transform.position.y - moveComponent.StartCardPosition.y;

            entity.RemoveComponent<InteractiveMoveComponent>();
            entity.RemoveComponent<InteractiveSelectCardComponent>();

            if (distance > 150)
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

                var view = _dataWorld.OneData<ViewPlayerData>();
                _dataWorld.RiseEvent(new EventUpdateBoardCard());
                _dataWorld.RiseEvent(new EventCardAnimationsHand { TargetPlayer = view.PlayerView });
            }
            else
            {
                InteractiveActionCard.ReturnAllCardInHand?.Invoke();
            }
        }

        private void EndMoveShopCard(Entity entity)
        {
            var componentMove = entity.GetComponent<InteractiveMoveComponent>();
            ref var componentCard = ref entity.GetComponent<CardComponent>();
            var distance = componentCard.Transform.position.y - componentMove.StartCardPosition.y;
            var roundPlayer = _dataWorld.OneData<RoundData>();

            if (distance < -50)
            {
                Debug.LogError("Buy card");
                ref var actionValue = ref _dataWorld.OneData<AbilityData>();
                actionValue.SpendTrade += componentCard.Price;
                entity.RemoveComponent<CardTradeRowComponent>();

                if (entity.HasComponent<CardComponentAnimations>())
                {
                    var animationCard = entity.GetComponent<CardComponentAnimations>();
                    animationCard.Sequence.Kill();
                    entity.RemoveComponent<CardComponentAnimations>();
                }

                componentCard.Player = roundPlayer.CurrentPlayer;
                entity.AddComponent(new CardMoveToDiscardComponent());
                _dataWorld.RiseEvent(new EventUpdateBoardCard());
            }
            else
            {
                var card = entity.GetComponent<CardComponent>();
                card.Transform.position = componentMove.StartCardPosition;
            }

            entity.RemoveComponent<InteractiveSelectCardComponent>();
            entity.RemoveComponent<InteractiveMoveComponent>();
        }

        public void Destroy()
        {
            InteractiveActionCard.StartInteractiveCard -= DownClickCard;
            InteractiveActionCard.EndInteractiveCard -= UpClickCard;
        }
    }
}