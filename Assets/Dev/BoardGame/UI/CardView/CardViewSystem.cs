using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using ModulesFrameworkUnity;
using System.Collections.Generic;
using UnityEngine;

namespace BoardGame.Core.UI
{
    [EcsSystem(typeof(BoardGameModule))]
    public class CardViewSystem : IInitSystem, IPostRunEventSystem<EventViewCard>
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            var config = _dataWorld.OneData<BoardGameData>().BoardGameConfig;
            var go = Object.Instantiate(config.ViewCardCanvas);
            go.transform.SetAsLastSibling();
            go.SetActive(false);
            _dataWorld.CreateOneData(new CardViewData { GO = go });

            InteractiveActionCard.HideViewCard += HideViewCard;
        }

        public void PostRunEvent(EventViewCard value) => ShowViewCard(value.TargetCard);

        private void ShowViewCard(CardMono card)
        {
            var cardView = _dataWorld.OneData<CardViewData>().GO;

            var canvasCard = Object.Instantiate(card.Canvas, cardView.transform);
            var rect = canvasCard.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector3.zero;
            canvasCard.transform.rotation = Quaternion.identity;
            canvasCard.transform.localScale = new Vector3(2, 2, 1);
            var cardMono = canvasCard.GetComponent<InteractiveCardMono>();
            Component.Destroy(cardMono);

            var newEntity = EcsWorldContainer.World.NewEntity();
            newEntity.AddComponent(new CardShowViewComponent { GO = canvasCard.gameObject });

            cardView.SetActive(true);
        }

        private void HideViewCard()
        {
            var entity = _dataWorld.Select<CardShowViewComponent>().SelectFirstEntity();
            var component = entity.GetComponent<CardShowViewComponent>();

            _dataWorld.OneData<CardViewData>().GO.SetActive(false);

            Object.Destroy(component.GO);
            entity.Destroy();
        }
    }
}