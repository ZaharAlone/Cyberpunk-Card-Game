using BoardGame.Core.UI;
using EcsCore;
using Input;
using ModulesFrameworkUnity;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace BoardGame.Core
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

            if (entity.HasComponent<CardHandComponent>() || entity.HasComponent<CardFreeToBuyComponent>())
            {
                ref var inputData = ref _dataWorld.OneData<InputData>();
                ref var component = ref entity.GetComponent<CardComponent>();
                entity.AddComponent(new InteractiveMoveComponent
                {
                    StartCardPosition = component.Transform.position,
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
            var entity = _dataWorld.Select<InteractiveMoveComponent>().SelectFirstEntity();

            if (entity.HasComponent<CardPlayer1Component>() || entity.HasComponent<CardPlayer2Component>())
                EndMovePlayerCard(entity);
            else if (entity.HasComponent<CardTradeRowComponent>())
                EndMoveShopCard(entity);
        }

        private void EndMovePlayerCard(Entity entity)
        {
            var componentMove = entity.GetComponent<InteractiveMoveComponent>();
            var componentCard = entity.GetComponent<CardComponent>();
            var distance = componentCard.Transform.position.y - componentMove.StartCardPosition.y;

            if (distance > 150)
            {
                entity.RemoveComponent<CardHandComponent>();
                entity.AddComponent(new CardTableComponent());
                _dataWorld.RiseEvent(new EventUpdateBoardCard());
            }
            else
            {
                var card = entity.GetComponent<CardComponent>();
                var pos = _dataWorld.OneData<BoardGameData>().BoardGameConfig.PlayerHandPosition;
                card.Transform.position = pos;
            }

            entity.RemoveComponent<InteractiveMoveComponent>();
            _dataWorld.RiseEvent(new EventUpdateHandUI());
        }

        private void EndMoveShopCard(Entity entity)
        {
            var componentMove = entity.GetComponent<InteractiveMoveComponent>();
            var componentCard = entity.GetComponent<CardComponent>();
            var distance = componentCard.Transform.position.y - componentMove.StartCardPosition.y;

            if (distance < -50)
            {
                ref var actionValue = ref _dataWorld.OneData<ActionData>();
                actionValue.SpendTrade += componentCard.Price;
                entity.RemoveComponent<CardTradeRowComponent>();
                entity.AddComponent(new CardPlayer1Component());
                entity.AddComponent(new CardDiscardComponent ());
                _dataWorld.RiseEvent(new EventUpdateBoardCard());
            }
            else
            {
                var card = entity.GetComponent<CardComponent>();
                card.Transform.position = componentMove.StartCardPosition;
            }

            entity.RemoveComponent<InteractiveMoveComponent>();
        }

        public void Destroy()
        {
            InteractiveActionCard.StartInteractiveCard -= DownClickCard;
            InteractiveActionCard.EndInteractiveCard -= UpClickCard;
        }
    }
}