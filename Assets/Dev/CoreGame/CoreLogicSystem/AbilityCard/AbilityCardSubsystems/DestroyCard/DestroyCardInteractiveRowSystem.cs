using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using CyberNet.Core.UI;
using CyberNet.Global.GameCamera;
using DG.Tweening;
using Input;

namespace CyberNet.Core.AbilityCard.DestroyCard
{
    /// <summary>
    /// Система для взаимодействия с картой в абилки уничтожения карты
    /// Позволяет перемещать и взаимодействовать с картой в ряду, отправлять её на уничтожение
    /// или возвращать обратно.
    /// </summary>
    [EcsSystem(typeof(CoreModule))]
    public class DestroyCardInteractiveRowSystem : IPreInitSystem, IRunSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        private const float card_select_animations_move_delta_y = -30f;
        private const float time_select_animations = 0.1f;
        private const float time_animations_return_to_row = 0.2f;
        private Vector3 card_select_animations_scale = new Vector3(1.15f, 1.15f, 1.15f);

        private const int base_layer_card = 310;
        private const int move_layer_card = 311;
        
        public void PreInit()
        {
            DestroyCardAction.SelectCard += SelectCard;
            //DestroyCardAction.CheckCardIsDestroy += CheckCardIsDestroy;
            DestroyCardAction.DeselectCard += DeselectCard;
            DestroyCardAction.StartMoveCard += StartMoveCard;
            DestroyCardAction.EndMoveCard += EndMoveCard;
        }
        
        private void SelectCard(string guidCard)
        {
            var isMoveCard = _dataWorld.Select<DestroyCardMoveComponent>()
                .Where<DestroyCardMoveComponent>(card => card.GUID == guidCard)
                .Count() > 0;
            
            if (isMoveCard)
                return;
            
            ref var destroyCardRow = ref _dataWorld.OneData<DestroyRowCardData>().DestroyCardInRow;
            destroyCardRow.TryGetValue(guidCard, out var cardElement);
            var cardRect = cardElement.CardMono.RectTransform;
            
            cardElement.StartPosition = cardRect.anchoredPosition;
            cardElement.StartScale = cardRect.localScale;
            
            cardElement.Sequence = DOTween.Sequence();
            var targetPositions = cardRect.anchoredPosition;
            targetPositions.y += card_select_animations_move_delta_y;
            
            cardElement.Sequence.Join(cardRect.DOScale(card_select_animations_scale, time_select_animations))
                .Join(cardRect.DOAnchorPos(targetPositions, time_select_animations));
            
            destroyCardRow[guidCard] = cardElement;
            
            //TODO Show popup for destroy card
            //CoreElementInfoPopupAction.OpenPopupCard?.Invoke(guidCard, false);
        }

        private void DeselectCard(string guidCard)
        {
            ref var destroyCardRow = ref _dataWorld.OneData<DestroyRowCardData>().DestroyCardInRow;
            destroyCardRow.TryGetValue(guidCard, out var cardElement);

            var isMoveCard = _dataWorld.Select<DestroyCardMoveComponent>()
                .Where<DestroyCardMoveComponent>(card => card.GUID == guidCard)
                .Count() > 0;
            
            if (isMoveCard)
                return;
            
            var cardRect = cardElement.CardMono.RectTransform;
            cardElement.Sequence.Kill();
            
            cardElement.Sequence = DOTween.Sequence();
            cardElement.Sequence.Join(cardRect.DOScale(cardElement.StartScale, time_select_animations))
                .Join(cardRect.DOAnchorPos(cardElement.StartPosition, time_select_animations));
            
            //TODO close popup
            //CoreElementInfoPopupAction.ClosePopupCard?.Invoke();
        }

        private void StartMoveCard(string guidCard)
        {
            var destroyCardRow = _dataWorld.OneData<DestroyRowCardData>().DestroyCardInRow;
            destroyCardRow.TryGetValue(guidCard, out var cardElement);

            var mousePosition = InputAction.GetCurrentMousePositionsToScreen.Invoke();;
            var entityMoveCard = _dataWorld.NewEntity();
            entityMoveCard.AddComponent(new DestroyCardMoveComponent {
                GUID = guidCard,
                PrevMousePosition = mousePosition,
                DestroyCardStruct = cardElement,
            });

            cardElement.CardMono.Canvas.sortingOrder = move_layer_card;
            ControlRaycastInteractiveCard(false, guidCard);
        }

        public void Run()
        {
            var countEntityMove = _dataWorld.Select<DestroyCardMoveComponent>().Count();
            if (countEntityMove != 0)
                MoveCard();
        }

        private void MoveCard()
        {
            var countEntityMove = _dataWorld.Select<DestroyCardMoveComponent>().Count();
            if (countEntityMove == 0)
                return;

            var moveCardEntity = _dataWorld.Select<DestroyCardMoveComponent>()
                .SelectFirstEntity();
            ref var moveCardComponent = ref moveCardEntity.GetComponent<DestroyCardMoveComponent>();
            var cardElement = moveCardComponent.DestroyCardStruct;
            
            var mousePosition = InputAction.GetCurrentMousePositionsToScreen.Invoke();;
            var deltaPosition = mousePosition - moveCardComponent.PrevMousePosition;
            moveCardComponent.PrevMousePosition = mousePosition;
            cardElement.CardMono.RectTransform.anchoredPosition += deltaPosition;
            
            var targetDestroyZone = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.DestroyCardUIMono.TransformDestroyZone;
            
            var distanceToDestroyZone = Vector2.Distance(cardElement.CardMono.RectTransform.position, targetDestroyZone.position);
            
            if (distanceToDestroyZone < 200 && !moveCardEntity.HasComponent<DestroyCardReadyComponent>())
            {
                moveCardEntity.AddComponent(new DestroyCardReadyComponent());
                cardElement.InteractiveDestroyCardMono.OnDestroyCardEffect();
            }
            else if (distanceToDestroyZone >= 200 && moveCardEntity.HasComponent<DestroyCardReadyComponent>())
            {
                moveCardEntity.RemoveComponent<DestroyCardReadyComponent>();
                cardElement.InteractiveDestroyCardMono.OffDestroyCardEffect();
            }
        }

        private void EndMoveCard(string guidCard)
        {
            ref var destroyCardRow = ref _dataWorld.OneData<DestroyRowCardData>().DestroyCardInRow;
            destroyCardRow.TryGetValue(guidCard, out var cardElement);
            var cardRect = cardElement.CardMono.RectTransform;
            var moveDestroyCardEntity = _dataWorld.Select<DestroyCardMoveComponent>().SelectFirstEntity();
            
            if (moveDestroyCardEntity.HasComponent<DestroyCardReadyComponent>())
            {
                var uiDestroyRow = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.DestroyCardUIMono;
                cardElement.CardMono.gameObject.transform.SetParent(uiDestroyRow.ContainerForDestroyCard);
                
                cardElement.InteractiveDestroyCardMono.OffDestroyCardEffect();
                cardElement.InteractiveDestroyCardMono.DisableInteractive();
                moveDestroyCardEntity.Destroy();
                
                DestroyCardAction.SelectCardToDestroy?.Invoke(guidCard);
            }
            else
            {
                moveDestroyCardEntity.Destroy();
                cardElement.Sequence.Kill();
                cardElement.CardMono.GraphicRaycaster.enabled = false;
                cardElement.CardMono.Canvas.sortingOrder = base_layer_card;
                cardElement.InteractiveDestroyCardMono.DisableSelectCard();
                
                cardElement.Sequence = DOTween.Sequence();
                cardElement.Sequence.Join(cardRect.DOScale(cardElement.StartScale, time_animations_return_to_row))
                    .Join(cardRect.DOAnchorPos(cardElement.StartPosition, time_animations_return_to_row))
                    .OnComplete(()=> ControlRaycastInteractiveCard(true));   
            }
        }

        private void ControlRaycastInteractiveCard(bool isEnableRaycast, string guidCard = "")
        {
            var destroyCardRow = _dataWorld.OneData<DestroyRowCardData>().DestroyCardInRow;
            
            foreach (var cardInRow in destroyCardRow)
            {
                if (cardInRow.Key != guidCard)
                    cardInRow.Value.CardMono.GraphicRaycaster.enabled = isEnableRaycast;
            }
        }

        public void Destroy()
        {
            DestroyCardAction.SelectCard -= SelectCard;
            DestroyCardAction.DeselectCard -= DeselectCard;
            DestroyCardAction.StartMoveCard -= StartMoveCard;
            DestroyCardAction.EndMoveCard -= EndMoveCard;
        }
    }
}