using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems.Events;
using ModulesFramework.Data.Enumerators;
using UnityEngine;
using System;
using DG.Tweening;
//TODO: вернуть

namespace CyberNet.Core.UI
{
    /// <summary>
    /// Анимация карт - расположение в руке полукругом
    /// </summary>
    [EcsSystem(typeof(CoreModule))]
    public class AnimationsFanCardInHandSystem : IPostRunEventSystem<EventCardAnimationsHand>
    {
        private DataWorld _dataWorld;

        public void PostRunEvent(EventCardAnimationsHand value)
        {
            var countCardInHand = _dataWorld.Select<CardComponent>()
                                .Where<CardComponent>(card => card.Player == value.TargetPlayer)
                                .With<CardHandComponent>()
                                .Without<CardTableComponent>()
                                .Without<WaitAnimationsDrawHandCardComponent>()
                                .Count();

            var entities = _dataWorld.Select<CardComponent>()
                                     .Where<CardComponent>(card => card.Player == value.TargetPlayer)
                                     .With<CardHandComponent>()
                                     .Without<CardTableComponent>()
                                     .Without<WaitAnimationsDrawHandCardComponent>()
                                     .GetEntities();
            
            UpdateView(entities, countCardInHand, value.TargetPlayer);
        }

        private void UpdateView(EntitiesEnumerable entities, int countCard, PlayerEnum isPlayer)
        {
            var viewPlayer = _dataWorld.OneData<ViewPlayerData>();
            var uiRect = _dataWorld.OneData<CoreUIData>().BoardGameUIMono.UIRect;
            var config = _dataWorld.OneData<BoardGameData>().BoardGameConfig;

            var screenShift = 0f;
            var length = 0f;
            var multPosY = 0;
            var radius = 0f;
            var multiplieSizeCard = Vector3.zero;

            if (viewPlayer.PlayerView == isPlayer)
            {
                screenShift = uiRect.rect.height / 2 - 125;
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
                cardComponent.RectTransform.localScale = multiplieSizeCard;
                length += 204 * cardComponent.RectTransform.localScale.x;
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

                    if (entity.HasComponent<CardComponentAnimations>() && entity.HasComponent<CardDistributionComponent>())
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
            animationComponent.Sequence = DOTween.Sequence();
            animationComponent.Sequence.Append(cardComponent.RectTransform.DOAnchorPos(position, 0.4f))
                                       .Join(cardComponent.RectTransform.DOLocalRotateQuaternion(rotate, 0.4f))
                                       .OnComplete(() => ClearAnimationComponent(entity));
        }

        private void ClearAnimationComponent(Entity entity)
        {
            VFXCardInteractivAction.UpdateVFXCard?.Invoke();
            ref var animationComponent = ref entity.GetComponent<CardComponentAnimations>();
            animationComponent.Sequence.Kill();
            entity.RemoveComponent<CardComponentAnimations>();
        }
    }
}