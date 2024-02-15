using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using System.Threading.Tasks;
using CyberNet.Core.UI;
using DG.Tweening;
using Input;

namespace CyberNet.Core.AbilityCard.DestroyCard
{
    [EcsSystem(typeof(CoreModule))]
    public class DestroyRowInteractiveCardSystem : IPreInitSystem, IRunSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            DestroyCardAction.SelectCard += SelectCard;
            DestroyCardAction.DeselectCard += DeselectCard;
            DestroyCardAction.StartMoveCard += StartMoveCard;
            DestroyCardAction.EndMoveCard += EndMoveCard;
        }
        
        private void SelectCard(string guidCard)
        {
            ref var destroyCardRow = ref _dataWorld.OneData<DestroyRowCardData>().DestroyCardInRow;
            destroyCardRow.TryGetValue(guidCard, out var cardElement);
            var cardRect = cardElement.CardMono.RectTransform;

            cardElement.StartPosition = cardRect.anchoredPosition;
            cardElement.StartScale = cardRect.localScale;
            
            cardElement.Sequence = DOTween.Sequence();
            var targetPositions = cardRect.anchoredPosition;
            targetPositions.y += 30;
            
            cardElement.Sequence.Join(cardRect.DOScale(new Vector3(1.15f, 1.15f, 1.15f), 0.1f))
                .Join(cardRect.DOAnchorPos(targetPositions, 0.1f));
            
            destroyCardRow[guidCard] = cardElement;
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
            cardElement.Sequence.Join(cardRect.DOScale(cardElement.StartScale, 0.1f))
                .Join(cardRect.DOAnchorPos(cardElement.StartPosition, 0.1f));
        }

        private void StartMoveCard(string guidCard)
        {
            ref var inputData = ref _dataWorld.OneData<InputData>();
            ref var destroyCardRow = ref _dataWorld.OneData<DestroyRowCardData>().DestroyCardInRow;
            destroyCardRow.TryGetValue(guidCard, out var cardElement);
            
            var entityMoveCard = _dataWorld.NewEntity();
            entityMoveCard.AddComponent(new DestroyCardMoveComponent {
                GUID = guidCard,
                PrevMousePosition = inputData.MousePosition
            });
        }

        public void Run()
        {
            var countEntityMove = _dataWorld.Select<DestroyCardMoveComponent>().Count();
            if (countEntityMove > 0)
                MoveCard();
        }

        private void MoveCard()
        {
            ref var inputData = ref _dataWorld.OneData<InputData>();
            var moveCardEntity = _dataWorld.Select<DestroyCardMoveComponent>()
                .SelectFirstEntity();
            ref var moveCardComponent = ref moveCardEntity.GetComponent<DestroyCardMoveComponent>();
            
            ref var destroyCardRow = ref _dataWorld.OneData<DestroyRowCardData>().DestroyCardInRow;
            destroyCardRow.TryGetValue(moveCardComponent.GUID, out var cardElement);
            
            var deltaMove = inputData.MousePosition - moveCardComponent.PrevMousePosition;
            cardElement.CardMono.RectTransform.anchoredPosition += new Vector2(deltaMove.x, deltaMove.y);
            moveCardComponent.PrevMousePosition = inputData.MousePosition;

            var deltaDistance = Vector2.Distance(cardElement.StartPosition,cardElement.CardMono.RectTransform.anchoredPosition);
            if (deltaDistance >= 125 && !moveCardEntity.HasComponent<DestroyCardReadyComponent>())
            {
                moveCardEntity.AddComponent(new DestroyCardReadyComponent());
                cardElement.InteractiveDestroyCardMono.OnDestroyCardEffect();
            }
            else if (deltaDistance < 125 && moveCardEntity.HasComponent<DestroyCardReadyComponent>())
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
                cardElement.InteractiveDestroyCardMono.OffDestroyCardEffect();
                cardElement.InteractiveDestroyCardMono.DisableInteractive();
                moveDestroyCardEntity.Destroy();
                var centerScreen = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.DestroyCardUIMono.CenterScreen.localPosition;
                
                cardElement.Sequence = DOTween.Sequence();
                cardElement.Sequence.Join(cardRect.DOScale(cardElement.StartScale, 0.25f))
                    .Join(cardRect.DOLocalMove(centerScreen, 0.25f));   
                
                WaitEndMoveCard(0.25f, guidCard);
            }
            else
            {
                moveDestroyCardEntity.Destroy();
                cardElement.Sequence.Kill();
            
                cardElement.Sequence = DOTween.Sequence();
                cardElement.Sequence.Join(cardRect.DOScale(cardElement.StartScale, 0.25f))
                    .Join(cardRect.DOAnchorPos(cardElement.StartPosition, 0.25f));   
            }
        }

        private async void WaitEndMoveCard(float timeWait, string guidCard)
        {
            await Task.Delay((int)(timeWait * 10000));
            DestroyCard(guidCard);
        }

        private void DestroyCard(string guidCard)
        {
            
            DestroyCardAction.SelectCardToDestroy?.Invoke(guidCard);
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