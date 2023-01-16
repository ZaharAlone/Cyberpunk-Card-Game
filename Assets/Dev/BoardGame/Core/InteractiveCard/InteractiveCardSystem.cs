using BoardGame.Core.UI;
using EcsCore;
using Input;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace BoardGame.Core
{
    [EcsSystem(typeof(BoardGameModule))]
    public class InteractiveCardSystem : IInitSystem, IRunSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            InteractiveActionCard.InteractiveCard += InteractiveCard;
        }

        private void InteractiveCard(bool isActive, string guid, string key)
        {
            var entities = _dataWorld.Select<CardComponent>().Where<CardComponent>(card => card.GUID == guid).GetEntities();

            foreach (var entity in entities)
            {
                Interactive(isActive, entity, key);
                break;
            }
        }

        public void Interactive(bool isActive, Entity entity, string key)
        {
            ref var component = ref entity.GetComponent<CardComponent>();

            if (key == "Left")
            {
                if (isActive)
                {
                    ref var inputData = ref _dataWorld.OneData<InputData>();
                    entity.AddComponent(new InteractiveMoveComponent
                    {
                        StartCardPosition = component.Transform.position,
                        StartMousePositions = inputData.MousePosition
                    });
                }
                else
                {
                    CheckEndPosition(entity);
                    entity.RemoveComponent<InteractiveMoveComponent>();
                }
            }

            if (isActive && key == "Right")
            {
                component.CardMono.SwitchFaceCard();
            }
        }

        public void Run()
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

        private void CheckEndPosition(Entity entity)
        {
            var componentMove = entity.GetComponent<InteractiveMoveComponent>();
            var componentCard = entity.GetComponent<CardComponent>();
            var distance = componentCard.Transform.position.y - componentMove.StartCardPosition.y;

            if (distance > 150)
            {
                entity.RemoveComponent<CardInHandComponent>();
                entity.AddComponent(new CardInDeckComponent());
                _dataWorld.CreateOneFrame().AddComponent(new EventUpdateBoardCardUI());
            }
            else
            {
                var card = entity.GetComponent<CardComponent>();
                var pos = _dataWorld.OneData<BoardGameData>().BoardGameConfig.PlayerHandPosition;
                card.Transform.position = pos;
            }
            _dataWorld.CreateOneFrame().AddComponent(new EventUpdateHandUI());
        }

        public void Destroy()
        {
            InteractiveActionCard.InteractiveCard -= InteractiveCard;
        }
    }
}