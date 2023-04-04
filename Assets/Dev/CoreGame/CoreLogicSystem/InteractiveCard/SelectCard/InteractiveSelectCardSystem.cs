using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using ModulesFrameworkUnity;
using System.Collections.Generic;
using UnityEngine;

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
            var entity = _dataWorld.Select<CardComponent>()
                        .Where<CardComponent>(card => card.GUID == guid)
                        .SelectFirstEntity();

            ref var view = ref _dataWorld.OneData<ViewPlayerData>();

            if (view.PlayerView == PlayerEnum.Player1 && entity.HasComponent<CardPlayer2Component>())
                return;
            else if (view.PlayerView == PlayerEnum.Player2 && entity.HasComponent<CardPlayer1Component>())
                return;

                //if (!entity.HasComponent<CardHandComponent>())
                //    return;

            ref var cardComponent = ref entity.GetComponent<CardComponent>();
            entity.AddComponent(new InteractiveSelectCardComponent { Positions = cardComponent.Transform.position, Rotate = cardComponent.Transform.rotation, SortingOrder = cardComponent.Canvas.sortingOrder });
            cardComponent.Transform.rotation = Quaternion.identity;
            cardComponent.Transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
            cardComponent.Canvas.sortingOrder = 20;

            if (entity.HasComponent<CardPlayer1Component>() || entity.HasComponent<CardPlayer2Component>())
            {
                var pos = cardComponent.Transform.localPosition;
                pos.y = -340;
                cardComponent.Transform.localPosition = pos;
            }
        }

        private void DeselectCard()
        {
            var entityCount = _dataWorld.Select<CardComponent>()
                                        .With<InteractiveSelectCardComponent>()
                                        .Count();

            if (entityCount == 0)
                return;

            var entity = _dataWorld.Select<CardComponent>()
                        .With<InteractiveSelectCardComponent>()
                        .SelectFirstEntity();

            //if (!entity.HasComponent<CardHandComponent>())
            //    return;

            ref var cardComponent = ref entity.GetComponent<CardComponent>();
            ref var selectComponent = ref entity.GetComponent<InteractiveSelectCardComponent>();
            cardComponent.Transform.rotation = selectComponent.Rotate;
            cardComponent.Transform.position = selectComponent.Positions;
            cardComponent.Transform.localScale = Vector3.one;
            cardComponent.Canvas.sortingOrder = selectComponent.SortingOrder;
            entity.RemoveComponent<InteractiveSelectCardComponent>();
        }
    }
}