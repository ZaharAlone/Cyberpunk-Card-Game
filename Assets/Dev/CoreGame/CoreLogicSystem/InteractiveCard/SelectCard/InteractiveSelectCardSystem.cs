using DG.Tweening;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class InteractiveSelectCardSystem : IInitSystem
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            InteractiveActionCard.SelectCard += SelectCard;
            InteractiveActionCard.DeselectCard += DeselectCard;
            InteractiveActionCard.ReturnAllCardInHand += ReturnAllCard;
        }

        private void SelectCard(string guid)
        {
            if (_dataWorld.Select<InteractiveSelectCardComponent>().Count() != 0)
                return;

            var isEntity = _dataWorld.Select<CardComponent>()
                        .Where<CardComponent>(card => card.GUID == guid)
                        .Without<CardTableComponent>()
                        .Without<CardDrawComponent>()
                        .Without<CardDistributionComponent>()
                        .TrySelectFirstEntity(out var entity);

            if (!isEntity)
                return;

            ref var cardComponent = ref entity.GetComponent<CardComponent>();
            ref var view = ref _dataWorld.OneData<ViewPlayerData>();

            if (view.PlayerView != cardComponent.Player && !entity.HasComponent<CardTradeRowComponent>())
                return;
            ClearSelectComponent();
            entity.AddComponent(new InteractiveSelectCardComponent());

            var animComponent = new CardComponentAnimations();
            if (entity.HasComponent<CardComponentAnimations>())
            {
                animComponent = entity.GetComponent<CardComponentAnimations>();
                animComponent.Sequence.Kill();
            }
            else
            {
                animComponent.Positions = cardComponent.RectTransform.position;
                animComponent.Rotate = cardComponent.RectTransform.rotation;
                animComponent.Scale = cardComponent.RectTransform.localScale;
                animComponent.SortingOrder = cardComponent.Canvas.sortingOrder;
            }

            var gameConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig;
            var scaleCard = gameConfig.SizeSelectCardHand;
            if (entity.HasComponent<CardTradeRowComponent>())
                scaleCard = gameConfig.SizeSelectCardTradeRow;

            animComponent.Sequence = DOTween.Sequence();
            animComponent.Sequence.Append(cardComponent.RectTransform.DORotateQuaternion(Quaternion.identity, 0.15f))
                                  .Join(cardComponent.RectTransform.DOAnchorPos(scaleCard, 0.15f));

            cardComponent.Canvas.sortingOrder = 20;

            if (cardComponent.Player != PlayerEnum.None)
            {
                var pos = animComponent.Positions;
                pos.y = -340;
                animComponent.Sequence.Join(cardComponent.RectTransform.DOMove(pos, 0.15f));
                entity.AddComponent(animComponent);
                var index = entity.GetComponent<CardSortingIndexComponent>().Index;
                MoveOtherCards(index);
            }
            else
                entity.AddComponent(animComponent);
        }

        private void ClearSelectComponent()
        {
            var entities = _dataWorld.Select<CardComponent>()
                            .With<InteractiveSelectCardComponent>()
                            .GetEntities();

            foreach (var entity in entities)
                entity.RemoveComponent<InteractiveSelectCardComponent>();
        }

        private void MoveOtherCards(int targetIndex)
        {
            var view = _dataWorld.OneData<ViewPlayerData>();
            var entities = _dataWorld.Select<CardComponent>()
                                     .Where<CardComponent>(card => card.Player == view.PlayerView)
                                     .With<CardHandComponent>()
                                     .With<CardSortingIndexComponent>()
                                     .Without<InteractiveSelectCardComponent>()
                                     .GetEntities();

            foreach (var entity in entities)
            {
                ref var index = ref entity.GetComponent<CardSortingIndexComponent>().Index;
                ref var cardComponent = ref entity.GetComponent<CardComponent>();
                var cardAnimations = new CardComponentAnimations();

                if (entity.HasComponent<CardComponentAnimations>())
                {
                    cardAnimations = entity.GetComponent<CardComponentAnimations>();
                    cardAnimations.Sequence.Kill();
                }
                else
                {
                    cardAnimations.Positions = cardComponent.RectTransform.position;
                    cardAnimations.Rotate = cardComponent.RectTransform.rotation;
                    cardAnimations.Scale = cardComponent.RectTransform.localScale;
                    cardAnimations.SortingOrder = cardComponent.Canvas.sortingOrder;
                }
                cardAnimations.Sequence = DOTween.Sequence();

                var targetPos = cardAnimations.Positions;
                if (index < targetIndex)
                {
                    if (index == targetIndex - 1)
                        targetPos.x -= 50;
                    else
                        targetPos.x -= 25;
                }
                else if (index > targetIndex)
                {
                    if (index == targetIndex + 1)
                        targetPos.x += 50;
                    else
                        targetPos.x += 25;
                }

                cardAnimations.Sequence.Append(cardComponent.RectTransform.DOAnchorPos(targetPos, 0.15f))
                                       .Join(cardComponent.RectTransform.DORotateQuaternion(cardAnimations.Rotate, 0.3f))
                                       .Join(cardComponent.RectTransform.DOScale(cardAnimations.Scale, 0.3f));

                cardComponent.Canvas.sortingOrder = cardAnimations.SortingOrder;
                entity.AddComponent(cardAnimations);
            }
        }

        private void DeselectCard(string guid)
        {
            if (_dataWorld.Select<InteractiveSelectCardComponent>().Count() > 1)
                return;

            var isEntity = _dataWorld.Select<CardComponent>()
                        .Where<CardComponent>(card => card.GUID == guid)
                        .With<InteractiveSelectCardComponent>()
                        .With<CardComponentAnimations>()
                        .Without<CardDistributionComponent>()
                        .Without<CardDrawComponent>()
                        .TrySelectFirstEntity(out var entity);

            if (!isEntity)
                return;

            entity.RemoveComponent<InteractiveSelectCardComponent>();
            if (entity.HasComponent<CardHandComponent>())
                ReturnAllCard();
            else
                ReturnCardAnimations(entity);
        }

        private void ReturnAllCard()
        {
            var view = _dataWorld.OneData<ViewPlayerData>();
            var entities = _dataWorld.Select<CardComponent>()
                                        .Where<CardComponent>(card => card.Player == view.PlayerView)
                                        .With<CardHandComponent>()
                                        .With<CardComponentAnimations>()
                                        .GetEntities();

            foreach (var entity in entities)
                ReturnCardAnimations(entity);
        }

        private void ReturnCardAnimations(Entity entity)
        {
            var index = entity.GetComponent<CardSortingIndexComponent>().Index;
            ref var cardComponent = ref entity.GetComponent<CardComponent>();
            ref var animationsCard = ref entity.GetComponent<CardComponentAnimations>();
            animationsCard.Sequence.Kill();
            animationsCard.Sequence = DOTween.Sequence();
            animationsCard.Sequence.Append(cardComponent.RectTransform.DORotateQuaternion(animationsCard.Rotate, 0.3f))
                                .Join(cardComponent.RectTransform.DOAnchorPos(animationsCard.Positions, 0.3f))
                                .Join(cardComponent.RectTransform.DOScale(animationsCard.Scale, 0.3f))
                                .OnComplete(() => FinishDeselect(entity));
        }

        private void FinishDeselect(Entity entity)
        {
            ref var cardComponent = ref entity.GetComponent<CardComponent>();
            var animationsCard = entity.GetComponent<CardComponentAnimations>();
            cardComponent.Canvas.sortingOrder = animationsCard.SortingOrder;
            entity.RemoveComponent<CardComponentAnimations>();
        }
    }
}