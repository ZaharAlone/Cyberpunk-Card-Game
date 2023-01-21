using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using UnityEngine;

namespace BoardGame.Core.UI
{
    [EcsSystem(typeof(BoardGameModule))]
    public class HandUISystem : IPostRunEventSystem<EventUpdateHandUI>
    {
        private DataWorld _dataWorld;

        public void PostRunEvent(EventUpdateHandUI _) => UpdateUI();

        private void UpdateUI()
        {
            var countCardInPlayerHand = _dataWorld.Select<CardPlayerComponent>().With<CardInHandComponent>().Count();
            var entities = _dataWorld.Select<CardPlayerComponent>().With<CardInHandComponent>().GetEntities();

            foreach (var entity in entities)
            {
                var pos = _dataWorld.OneData<BoardGameData>().BoardGameConfig.PlayerHandPosition;
                ref var card = ref entity.GetComponent<CardComponent>();
                card.Transform.rotation = Quaternion.identity;
                card.Transform.position = pos;
            }

            var stepAngle = 10f;
            var stepPosX = 170f;
            var angle = (float)(countCardInPlayerHand - 1) / 2 * (stepAngle);
            var posX = (float)(countCardInPlayerHand - 1) / 2 * (-stepPosX);

            var index = 0;
            foreach (var entity in entities)
            {
                ref var cardComponent = ref entity.GetComponent<CardComponent>();

                cardComponent.Transform.position = new Vector3(cardComponent.Transform.position.x + posX,
                                                               cardComponent.Transform.position.y,
                                                               cardComponent.Transform.position.z);

                cardComponent.Transform.eulerAngles = new Vector3(cardComponent.Transform.eulerAngles.x,
                                                                  cardComponent.Transform.eulerAngles.y,
                                                                  cardComponent.Transform.eulerAngles.z + angle);
                cardComponent.Canvas.sortingOrder = 2 + index;
                index++;

                posX += stepPosX;
                angle -= stepAngle;
            }
        }
    }
}