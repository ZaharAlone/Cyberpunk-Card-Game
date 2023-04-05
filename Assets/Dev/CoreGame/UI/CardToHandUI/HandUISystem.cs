using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using ModulesFramework.Data.Enumerators;
using UnityEngine;
using System;

namespace BoardGame.Core.UI
{
    [EcsSystem(typeof(CoreModule))]
    public class HandUISystem : IPostRunEventSystem<EventUpdateHandUI>
    {
        private DataWorld _dataWorld;

        public void PostRunEvent(EventUpdateHandUI value)
        {
            var countCardInHand = _dataWorld.Select<CardComponent>()
                                .Where<CardComponent>(card => card.Player == value.TargetPlayer)
                                .With<CardHandComponent>()
                                .Count();
            var entities = _dataWorld.Select<CardComponent>()
                                     .Where<CardComponent>(card => card.Player == value.TargetPlayer)
                                     .With<CardHandComponent>()
                                     .GetEntities();

            var minIndex = 1000;
            foreach (var entity in entities)
            {
                var indexComponent = entity.GetComponent<CardSortingIndexComponent>().Index;
                if (indexComponent < minIndex)
                    minIndex = indexComponent;
            }

            UpdateView(entities, countCardInHand, value.TargetPlayer, minIndex);
        }

        private void UpdateView(EntitiesEnumerable entities, int countCard, PlayerEnum isPlayer, int minIndex)
        {
            var viewPlayer = _dataWorld.OneData<ViewPlayerData>();
            var uiRect = _dataWorld.OneData<UIData>().UIMono.UIRect;

            var screenShift = 0f;
            var length = 0f;
            var multPosY = 0;
            var radius = 0;

            if (viewPlayer.PlayerView == isPlayer)
            {
                screenShift = uiRect.rect.height / 2 - 120;
                multPosY = -1;
                radius = 4000;
            }
            else
            {
                screenShift = -uiRect.rect.height / 2 + 70;
                multPosY = 1;
                radius = 1000;
            }

            foreach (var entity in entities)
            {
                ref var cardComponent = ref entity.GetComponent<CardComponent>();
                length += 204 * cardComponent.Transform.localScale.x;
            }

            var sizeCard = length / countCard;
            length /= 2;
            var height = (float)Math.Sqrt(radius * radius - length * length);

            var heightOne = (radius - height) / countCard * 2;
            var deltaHeight = radius - height - heightOne / 2;
            var deltaLength = length - (float)sizeCard / 2;

            var maxAngle = 90 - Mathf.Atan(height / length) * Mathf.Rad2Deg;
            float oneAngle = maxAngle / countCard;
            maxAngle -= oneAngle;
            var indexCard = 0;

            for (int i = 0; i < countCard; i++)
            {
                foreach (var entity in entities)
                {
                    ref var sortingIndexCard = ref entity.GetComponent<CardSortingIndexComponent>().Index;
                    if (sortingIndexCard != minIndex)
                        continue;

                    ref var cardComponent = ref entity.GetComponent<CardComponent>();
                    float angle = maxAngle / 2 - oneAngle * indexCard;
                    var posY = Mathf.Abs(deltaHeight - heightOne * indexCard) * multPosY;
                    var posX = sizeCard * indexCard - deltaLength;

                    cardComponent.Transform.position = new Vector3(posX, posY - screenShift, 0f);
                    cardComponent.Transform.rotation = Quaternion.Euler(0, 0, angle * -multPosY);
                    cardComponent.Canvas.sortingOrder = 2 + indexCard;
                    indexCard++;
                    break;
                }
                minIndex++;
            }
        }
    }
}