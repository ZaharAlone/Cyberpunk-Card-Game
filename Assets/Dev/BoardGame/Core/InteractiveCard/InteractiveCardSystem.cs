using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFrameworkUnity;
using System.Collections.Generic;
using UnityEngine;
using Input;

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
            var entities = _dataWorld.Select<CardComponent>().GetEntities();

            foreach (var entity in entities)
            {
                if (entity.GetComponent<CardComponent>().GUID == guid)
                {
                    Interactive(isActive, entity, key);
                    break;
                }
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
                    entity.AddComponent(new InteractiveMoveComponent { StartPositions = inputData.MousePosition });
                }
                else
                    entity.RemoveComponent<InteractiveMoveComponent>();
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

                var deltaMove = inputData.MousePosition - componentMove.StartPositions;
                componentCard.Transform.position += new Vector3(deltaMove.x, deltaMove.y, 0);
                componentMove.StartPositions = inputData.MousePosition;
            }
        }

        public void Destroy()
        {
            InteractiveActionCard.InteractiveCard -= InteractiveCard;
        }
    }
}