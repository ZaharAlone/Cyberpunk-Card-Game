using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using Input;
using System;
using CyberNet.Core.UI;
using CyberNet.Global.GameCamera;

namespace CyberNet.Core.InteractiveCard
{
    [EcsSystem(typeof(CoreModule))]
    public class MoveCardSystem : IRunSystem
    {
        private DataWorld _dataWorld;

        public void Run()
        {
            var countEntityMove = _dataWorld.Select<InteractiveMoveComponent>().Count();
            if (countEntityMove != 0)
                MoveCard();
        }

        private void MoveCard()
        {
            var entityMoveCard = _dataWorld.Select<CardComponent>().With<InteractiveMoveComponent>().SelectFirstEntity();
            var mousePosition = InputAction.GetCurrentMousePositionsToScreen.Invoke();
            
            ref var moveCardComponent = ref entityMoveCard.GetComponent<InteractiveMoveComponent>();
            var cardComponent = entityMoveCard.GetComponent<CardComponent>();
                
            var deltaMove = mousePosition - moveCardComponent.StartMousePositions;
            if (deltaMove == Vector2.zero)
                return;
            
            cardComponent.RectTransform.anchoredPosition += new Vector2(deltaMove.x, deltaMove.y);
            moveCardComponent.StartMousePositions = mousePosition;
            
            UpdateViewCard();
        }

        private void UpdateViewCard()
        {
            var entityMoveCard = _dataWorld.Select<CardComponent>().With<InteractiveMoveComponent>()
                .SelectFirstEntity();

            var cardComponent = entityMoveCard.GetComponent<CardComponent>();
            
            var cardWorldCorners = new Vector3[4];
            cardComponent.RectTransform.GetWorldCorners(cardWorldCorners );
            var boardGameUI = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;
            
            if (entityMoveCard.HasComponent<CardTradeRowComponent>())
            {
                var upperPositionCard = (cardWorldCorners[1] + cardWorldCorners[2]) / 2;
                var shopZone = boardGameUI.TraderowMono.TraderowContainer;
                CheckCardInTargetZone(cardComponent, shopZone, upperPositionCard);
            }
            else
            {
                var bottomPositionCard = (cardWorldCorners[0] + cardWorldCorners[3]) / 2;
                var playerZoneTransform = boardGameUI.CoreHudUIMono.ZoneHandCard;
                CheckCardInTargetZone(cardComponent, playerZoneTransform, bottomPositionCard);
            }
        }

        private void CheckCardInTargetZone(CardComponent cardComponent, RectTransform targetZone, Vector3 positionCard)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(targetZone, positionCard ))
                cardComponent.CardMono.CardFaceMono.VFXCardReadyToInteractive();
            else
                cardComponent.CardMono.CardFaceMono.VFXCardInTargetZone();
        }
    }
}