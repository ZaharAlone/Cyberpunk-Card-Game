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
            if (value.TargetPlayer == PlayerEnum.Player1)
            {
                var countCardInHand = _dataWorld.Select<CardPlayer1Component>().With<CardHandComponent>().Count();
                var entities = _dataWorld.Select<CardPlayer1Component>().With<CardHandComponent>().GetEntities();
                UpdateView(entities, countCardInHand, PlayerEnum.Player1);
            }
            else
            {
                var countCardInHand = _dataWorld.Select<CardPlayer2Component>().With<CardHandComponent>().Count();
                var entities = _dataWorld.Select<CardPlayer2Component>().With<CardHandComponent>().GetEntities();
                UpdateView(entities, countCardInHand, PlayerEnum.Player2);
            }
        }

        private void UpdateView(EntitiesEnumerable entities, int countCard, PlayerEnum isPlayer)
        {
            var viewPlayer = _dataWorld.OneData<ViewPlayerData>();
            var uiRect = _dataWorld.OneData<UIData>().UIMono.UIRect;
            var config = _dataWorld.OneData<BoardGameData>().BoardGameConfig;

            var screenShift = 0f;
            var size = new Vector3();
            var multPosY = 0;
            var radius = 0;
            if (viewPlayer.PlayerView == isPlayer)
            {
                screenShift = uiRect.rect.height / 2 - 120;
                size = config.SizeCardPlayerDown;
                multPosY = -1;
                radius = 4000;
            }
            else
            {
                screenShift = -uiRect.rect.height / 2 + 70;
                size = config.SizeCardPlayerUp;
                multPosY = 1;
                radius = 1000;
            }

            var sizeCard = 200f * size.x;
            var length = sizeCard * countCard / 2;
            var height = (float)Math.Sqrt(radius * radius - length * length);

            var heightOne = (radius - height) / countCard * 2;
            var deltaHeight = radius - height - heightOne / 2;
            var deltaLength = length - (float)sizeCard / 2;

            var maxAngle = 90 - Mathf.Atan(height / length) * Mathf.Rad2Deg;
            float oneAngle = maxAngle / countCard;
            maxAngle -= oneAngle;
            var index = 0;

            foreach (var entity in entities)
            {
                ref var cardComponent = ref entity.GetComponent<CardComponent>();
                float angle = maxAngle / 2 - oneAngle * index;
                var posY = Mathf.Abs(deltaHeight - heightOne * index) * multPosY;
                var posX = sizeCard * index - deltaLength;

                cardComponent.Transform.position = new Vector3(posX, posY - screenShift, 0f);
                cardComponent.Transform.rotation = Quaternion.Euler(0, 0, angle * -multPosY);
                cardComponent.Transform.localScale = size;
                cardComponent.Canvas.sortingOrder = 2 + index;
                index++;
            }
        }
    }
}