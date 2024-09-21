using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using CyberNet.Core.AbilityCard.DiscardCard;
using CyberNet.Core.InteractiveCard;
using CyberNet.Core.Player;
using CyberNet.Core.UI;
using CyberNet.Core.UI.CorePopup;
using CyberNet.Core.UI.TaskPlayerPopup;
using CyberNet.Global.Sound;
using Input;

namespace CyberNet.Core.AbilityCard
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityPlayerDiscardCardSystem : IPreInitSystem, IRunSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AbilityCardAction.PlayerDiscardCard += PlayerDiscardCard;
        }
        
        private void PlayerDiscardCard()
        {
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();

            ref var discardCardComponent = ref playerEntity.GetComponent<PlayerEffectDiscardCardComponent>();
            playerEntity.AddComponent(new PlayerIsDiscardsCardComponent());
            Debug.LogError("Player Discard Card");

            var supportLocData = _dataWorld.OneData<BoardGameData>().SupportLocalize;

            if (discardCardComponent.Count == 1)
            {
                var header = supportLocData.HeaderDiscardOneCard.mTerm;
                var descr = supportLocData.DescrDiscardOneCard.mTerm;
                TaskPlayerPopupAction.OpenPopup?.Invoke(header, descr);
            }
            else
            {
                var header = supportLocData.HeaderDiscardManyCard.mTerm;
                var descr = supportLocData.DescrDiscardManyCard.mTerm;
                var paramValue = discardCardComponent.Count.ToString();
                
                TaskPlayerPopupAction.OpenPopupParam?.Invoke(header, descr, paramValue);
            }
            
            InteractiveActionCard.StartInteractiveCard += StartMoveCard;
            InteractiveActionCard.EndInteractiveCard += EndMoveInteractiveCard;

            //TODO  доделать логику сброса
        }

        private void StartMoveCard(string guidCard)
        {
            var mousePositions = InputAction.GetCurrentMousePositionsToScreen.Invoke();
            var cardEntity = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == guidCard)
                .SelectFirstEntity();
            var cardComponent = cardEntity.GetComponent<CardComponent>();
            
            cardEntity.AddComponent(new InteractiveMoveCardToDiscardComponent
            {
                StartCardPosition = cardComponent.RectTransform.anchoredPosition,
                StartMousePositions = mousePositions,
            });
            CoreElementInfoPopupAction.ClosePopupCard?.Invoke();

            var startMoveSFX = _dataWorld.OneData<SoundData>().Sound.StartInteractiveCard;
            SoundAction.PlaySound?.Invoke(startMoveSFX);

            _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.BlockRaycastPanel.SetActive(true);
        }

        public void Run()
        {
            if (_dataWorld.Select<InteractiveMoveCardToDiscardComponent>().Count() == 0)
                return;
            
            MoveCard();
        }
        
        private void MoveCard()
        {
            var entityMoveCard = _dataWorld.Select<CardComponent>().With<InteractiveMoveCardToDiscardComponent>().SelectFirstEntity();
            var mousePosition = InputAction.GetCurrentMousePositionsToScreen.Invoke();
            
            ref var moveCardComponent = ref entityMoveCard.GetComponent<InteractiveMoveCardToDiscardComponent>();
            var cardComponent = entityMoveCard.GetComponent<CardComponent>();
                
            var deltaMove = mousePosition - moveCardComponent.StartMousePositions;
            cardComponent.RectTransform.anchoredPosition += new Vector2(deltaMove.x, deltaMove.y);
            moveCardComponent.StartMousePositions = mousePosition;
            
            var distanceCardMove = CalculateCardDistanceMove();
            if (distanceCardMove > 140 && !moveCardComponent.IsEffectDiscardCard)
            {
                cardComponent.CardMono.CardFaceMono.EnableDiscardCardEffect(true);
            }
            else
                cardComponent.CardMono.CardFaceMono.EnableDiscardCardEffect(false);
        }

        private void EndMoveInteractiveCard()
        {
            var cardEntity = _dataWorld.Select<InteractiveMoveCardToDiscardComponent>().SelectFirstEntity();
            var distanceCardMove = CalculateCardDistanceMove();

            if (distanceCardMove > 140)
            {
                var cardComponent = cardEntity.GetComponent<CardComponent>();
                cardEntity.RemoveComponent<CardHandComponent>();
                
                cardEntity.AddComponent(new CardMoveToDiscardComponent());
                cardComponent.Canvas.sortingOrder = 2;

                AnimationsMoveAtDiscardDeckAction.AnimationsMoveAtDiscardDeck?.Invoke();
                CardAnimationsHandAction.AnimationsFanCardInHand?.Invoke();

                EndDiscardCard();
            }
            else
            {
                InteractiveActionCard.ReturnAllCardInHand?.Invoke();
                SoundAction.PlaySound?.Invoke(_dataWorld.OneData<SoundData>().Sound.CancelInteractiveCard);
            }
        }

        private float CalculateCardDistanceMove()
        {
            var cardEntity = _dataWorld.Select<InteractiveMoveCardToDiscardComponent>().SelectFirstEntity();
            
            var moveComponent = cardEntity.GetComponent<InteractiveMoveCardToDiscardComponent>();
            var cardComponent = cardEntity.GetComponent<CardComponent>();
            var distance = cardComponent.RectTransform.anchoredPosition.y - moveComponent.StartCardPosition.y;
            return distance;
        }

        private void EndDiscardCard()
        {
            InteractiveActionCard.StartInteractiveCard -= StartMoveCard;
            InteractiveActionCard.EndInteractiveCard -= EndMoveInteractiveCard;
            
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();

            playerEntity.RemoveComponent<PlayerEffectDiscardCardComponent>();
            playerEntity.RemoveComponent<PlayerIsDiscardsCardComponent>();
            
            TaskPlayerPopupAction.ClosePopup?.Invoke();
            RoundAction.StartTurn?.Invoke();
        }

        public void Destroy()
        {
            AbilityCardAction.PlayerDiscardCard -= PlayerDiscardCard;
            InteractiveActionCard.StartInteractiveCard -= StartMoveCard;
            InteractiveActionCard.EndInteractiveCard -= EndMoveInteractiveCard;
        }
    }
}