using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Data.Enumerators;
using UnityEngine;
using System;
using CyberNet.Core.Battle.TacticsMode.InteractiveCard;
using DG.Tweening;
using ModulesFramework.Systems;

namespace CyberNet.Core.UI
{
    /// <summary>
    /// Анимация карт - расположение в руке полукругом
    /// </summary>
    [EcsSystem(typeof(CoreModule))]
    public class AnimationsFanCardInHandSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            CardAnimationsHandAction.AnimationsFanCardInHand += AnimationsFanCardInHand;
            CardAnimationsHandAction.AnimationsFanCardInTacticsScreen += AnimationsFanCardInTacticsScreen;
        }
        
        private void AnimationsFanCardInHand()
        {
            var currentRoundPlayer = _dataWorld.OneData<RoundData>().CurrentPlayerID;
            var cardInHandQuery = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.PlayerID == currentRoundPlayer)
                .With<CardHandComponent>()
                .Without<CardAbilitySelectionCompletedComponent>()
                .Without<WaitAnimationsDrawHandCardComponent>();
            
            UpdateView(cardInHandQuery.GetEntities(), cardInHandQuery.Count());
        }

        private void AnimationsFanCardInTacticsScreen()
        {
            var cardInHandQuery = _dataWorld.Select<CardComponent>()
                .With<CardTacticsComponent>()
                .Without<CardSelectInTacticsScreenComponent>()
                .Without<CardMoveToTacticsScreenComponent>();

            Debug.LogError($"Card fan in tactics screen {cardInHandQuery.Count()}");
            UpdateView(cardInHandQuery.GetEntities(), cardInHandQuery.Count());
        }
        
        private void UpdateView(EntitiesEnumerable cardEntities, int countCard)
        {
            var config = _dataWorld.OneData<BoardGameData>().BoardGameConfig;

            var screenShift = Screen.height / 2 - 125;
            var length = 0f;
            var multiplyPosY = -1;
            var radius = 2500;
            var multiplySizeCard = config.SizeCardPlayerDown;

            foreach (var entity in cardEntities)
            {
                ref var cardComponent = ref entity.GetComponent<CardComponent>();
                cardComponent.RectTransform.localScale = multiplySizeCard;
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
                targetIndex = SelectMinIndex(cardEntities, targetIndex);

                foreach (var entity in cardEntities)
                {
                    ref var sortingIndexCard = ref entity.GetComponent<CardSortingIndexComponent>().Index;
                    if (sortingIndexCard != targetIndex)
                        continue;

                    if (entity.HasComponent<CardComponentAnimations>() && entity.HasComponent<CardDistributionComponent>())
                        continue;

                    ref var cardComponent = ref entity.GetComponent<CardComponent>();
                    float angle = maxAngle / 2 - oneAngle * i;
                    var targetPos = Vector3.zero;

                    targetPos.y = Mathf.Abs(deltaHeight - heightOne * i) * multiplyPosY - screenShift;
                    targetPos.x = sizeCard * i - deltaLength;
                    var targetRotate = Quaternion.Euler(0, 0, angle * -multiplyPosY);
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
            VFXCardInteractiveAction.UpdateVFXCard?.Invoke();
            ref var animationComponent = ref entity.GetComponent<CardComponentAnimations>();
            animationComponent.Sequence.Kill();
            entity.RemoveComponent<CardComponentAnimations>();
        }

        public void Destroy()
        {
            CardAnimationsHandAction.AnimationsFanCardInHand -= AnimationsFanCardInHand;
            CardAnimationsHandAction.AnimationsFanCardInTacticsScreen -= AnimationsFanCardInTacticsScreen;
        }
    }
}