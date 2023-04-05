using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using ModulesFrameworkUnity;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using BoardGame.Core.UI;

namespace BoardGame.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class InteractiveSelectCardSystem : IInitSystem
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            InteractiveActionCard.SelectCard += SelectCard;
            InteractiveActionCard.DeselectCard += DeselectCard;
        }

        private void SelectCard(string guid)
        {
            var isEntity = _dataWorld.Select<CardComponent>()
                        .Where<CardComponent>(card => card.GUID == guid)
                        .Without<CardTableComponent>()
                        .TrySelectFirstEntity(out var entity);

            if (!isEntity)
                return;

            ref var cardComponent = ref entity.GetComponent<CardComponent>();
            ref var view = ref _dataWorld.OneData<ViewPlayerData>();

            if (view.PlayerView != cardComponent.Player)
                return;

            entity.AddComponent(new InteractiveSelectCardComponent { Positions = cardComponent.Transform.position, Rotate = cardComponent.Transform.rotation, SortingOrder = cardComponent.Canvas.sortingOrder });

            var sequence = DOTween.Sequence();
            sequence.Append(cardComponent.Transform.DORotateQuaternion(Quaternion.identity, 0.15f))
                    .Join(cardComponent.Transform.DOScale(new Vector3(1.4f, 1.4f, 1.4f), 0.15f));

            cardComponent.Canvas.sortingOrder = 20;

            if (cardComponent.Player != PlayerEnum.None)
            {
                var pos = cardComponent.Transform.localPosition;
                pos.y = -340;
                sequence.Join(cardComponent.Transform.DOMove(pos, 0.15f));

                var index = entity.GetComponent<CardSortingIndexComponent>().Index;
                MoveOtherCards(index, true);
            }
        }

        private void MoveOtherCards(int targetIndex, bool isUndraw)
        {
            var view = _dataWorld.OneData<ViewPlayerData>();
            var entities = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.Player == view.PlayerView)
                .With<CardHandComponent>()
                .With<CardSortingIndexComponent>()
                .GetEntities();
            var mult = 1f;
            if (!isUndraw)
                mult = -1f;

            foreach (var entity in entities)
            {
                ref var index = ref entity.GetComponent<CardSortingIndexComponent>().Index;
                ref var cardComponent = ref entity.GetComponent<CardComponent>();
                var sequence = DOTween.Sequence();

                if (index < targetIndex)
                {
                    if (index == targetIndex - 1)
                        sequence.Join(cardComponent.Transform.DORotate(new Vector3(0, 0, 4f * mult), 0.15f))
                                .Join(cardComponent.Transform.DOMove(new Vector3(-30 * mult, -10 * mult, 0), 0.15f))
                                .SetRelative(true);
                    else
                        sequence.Join(cardComponent.Transform.DORotate(new Vector3(0, 0, 1f * mult), 0.15f))
                                .Join(cardComponent.Transform.DOMove(new Vector3(-15 * mult, -20 * mult, 0), 0.15f))
                                .SetRelative(true);
                }
                else if (index > targetIndex)
                {
                    if (index == targetIndex + 1)
                        sequence.Join(cardComponent.Transform.DORotate(new Vector3(0, 0, -4f * mult), 0.15f))
                                .Join(cardComponent.Transform.DOMove(new Vector3(30 * mult, -10 * mult, 0), 0.15f))
                                .SetRelative(true);
                    else
                        sequence.Join(cardComponent.Transform.DORotate(new Vector3(0, 0, -1f * mult), 0.15f))
                                .Join(cardComponent.Transform.DOMove(new Vector3(15 * mult, -20 * mult, 0), 0.15f))
                                .SetRelative(true);
                }

            }
        }

        private void DeselectCard(string guid)
        {
            var isEntity = _dataWorld.Select<CardComponent>()
                        .Where<CardComponent>(card => card.GUID == guid)
                        .With<InteractiveSelectCardComponent>()
                        .TrySelectFirstEntity(out var entity);

            if (!isEntity)
                return;

            var cardComponent = entity.GetComponent<CardComponent>();
            var selectComponent = entity.GetComponent<InteractiveSelectCardComponent>();
            var sequence = DOTween.Sequence();
            sequence.Append(cardComponent.Transform.DORotateQuaternion(selectComponent.Rotate, 0.3f))
                    .Join(cardComponent.Transform.DOMove(selectComponent.Positions, 0.3f))
                    .Join(cardComponent.Transform.DOScale(Vector3.one, 0.3f))
                    .OnComplete(() => ReturnSortingOrder(cardComponent.Canvas, selectComponent.SortingOrder));

            var index = entity.GetComponent<CardSortingIndexComponent>().Index;
            MoveOtherCards(index, false);

            entity.RemoveComponent<InteractiveSelectCardComponent>();
        }

        private void ReturnSortingOrder(Canvas canvas, int sortingOrder)
        {
            canvas.sortingOrder = sortingOrder;
        }
    }
}