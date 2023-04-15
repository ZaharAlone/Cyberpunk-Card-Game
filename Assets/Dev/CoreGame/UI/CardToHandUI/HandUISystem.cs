using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using ModulesFramework.Data.Enumerators;
using UnityEngine;
using System;
using DG.Tweening;

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
                                .Without<CardTableComponent>()
                                .Count();
            var entities = _dataWorld.Select<CardComponent>()
                                     .Where<CardComponent>(card => card.Player == value.TargetPlayer)
                                     .With<CardHandComponent>()
                                     .Without<CardTableComponent>()
                                     .GetEntities();

            UpdateView(entities, countCardInHand, value.TargetPlayer);
        }

        private void UpdateView(EntitiesEnumerable entities, int countCard, PlayerEnum isPlayer)
        {
            var viewPlayer = _dataWorld.OneData<ViewPlayerData>();
            var uiRect = _dataWorld.OneData<UIData>().UIMono.UIRect;
            var config = _dataWorld.OneData<BoardGameData>().BoardGameConfig;

            var screenShift = 0f;
            var length = 0f;
            var multPosY = 0;
            var radius = 0f;
            var multiplieSizeCard = Vector3.zero;

            if (viewPlayer.PlayerView == isPlayer)
            {
                screenShift = uiRect.rect.height / 2 - 120;
                multPosY = -1;
                radius = 2500;
                multiplieSizeCard = config.SizeCardPlayerDown;
            }
            else
            {
                screenShift = -uiRect.rect.height / 2 + 70;
                multPosY = 1;
                radius = 1500;
                multiplieSizeCard = config.SizeCardPlayerUp;
            }

            foreach (var entity in entities)
            {
                ref var cardComponent = ref entity.GetComponent<CardComponent>();
                cardComponent.Transform.localScale = multiplieSizeCard;
                length += 204 * cardComponent.Transform.localScale.x;
            }

            var sizeCard = length / countCard;
            length /= 2;
            var height = (float)Math.Sqrt(radius * radius - length * length);

            var heightOne = (radius - height) / countCard * 2;
            var deltaHeight = radius - height - heightOne / 2;
            var deltaLength = length - (float)sizeCard / 2;

            var maxAngle = 90 - Mathf.Atan(height / length - 2) * Mathf.Rad2Deg;
            float oneAngle = maxAngle / countCard;
            maxAngle -= oneAngle;
            var targetIndex = -1;

            for (int i = 0; i < countCard; i++)
            {
                targetIndex = SelectMinIndex(entities, targetIndex);

                foreach (var entity in entities)
                {
                    ref var sortingIndexCard = ref entity.GetComponent<CardSortingIndexComponent>().Index;
                    if (sortingIndexCard != targetIndex)
                        continue;
                    ref var cardComponent = ref entity.GetComponent<CardComponent>();
                    float angle = maxAngle / 2 - oneAngle * i;
                    var targetPos = Vector3.zero;

                    targetPos.y = Mathf.Abs(deltaHeight - heightOne * i) * multPosY - screenShift;
                    targetPos.x = sizeCard * i - deltaLength;
                    var targetRotate = Quaternion.Euler(0, 0, angle * -multPosY);
                    MoveCard(entity, targetPos, targetRotate);

                    cardComponent.Canvas.sortingOrder = 2 + i;
                    break;
                }
            }
        }

        private int SelectMinIndex(EntitiesEnumerable entities, int oldIndex)
        {
            var targetIndex = 1000;
            foreach (var entity in entities)
            {
                var indexComponent = entity.GetComponent<CardSortingIndexComponent>().Index;
                if (indexComponent > oldIndex && indexComponent < targetIndex)
                    targetIndex = indexComponent;
            }
            return targetIndex;
        }

        private void MoveCard(Entity entity, Vector3 position, Quaternion rotate)
        {
            var cardComponent = entity.GetComponent<CardComponent>();
            var animationComponent = new CardComponentAnimations();
            var round = _dataWorld.OneData<RoundData>();

            if (round.CurrentRound == 0 && round.CurrentTurn == 1)
            {
                cardComponent.Transform.position = position;
                cardComponent.Transform.rotation = rotate;
                return;
            }

            animationComponent.Sequence = DOTween.Sequence();
            animationComponent.Sequence.Append(cardComponent.Transform.DOMove(position, 0.3f))
                                       .Join(cardComponent.Transform.DORotateQuaternion(rotate, 0.3f))
                                       .OnComplete(() => ClearAnimationComponent(entity));
        }

        private void ClearAnimationComponent(Entity entity)
        {
            var animationComponent = new CardComponentAnimations();
            animationComponent.Sequence.Kill();
            entity.RemoveComponent<CardComponentAnimations>();
        }
    }
}