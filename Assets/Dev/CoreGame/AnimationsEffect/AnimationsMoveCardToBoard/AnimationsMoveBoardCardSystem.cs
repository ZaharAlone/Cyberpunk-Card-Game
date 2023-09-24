using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using System.Threading.Tasks;
using DG.Tweening;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.UI;
using ModulesFramework.Data.Enumerators;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class AnimationsMoveBoardCardSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;
        private Sequence _sequence;
        
        public void PreInit()
        {
            AnimationsMoveBoardCardAction.AnimationsMoveBoardCard += AnimationsMoveBoardCard;
            RoundAction.EndCurrentTurn += EndRound;
        }

        private void AnimationsMoveBoardCard()
        {
            var entities = _dataWorld.Select<CardComponent>().With<CardTableComponent>().GetEntities();
            var config = _dataWorld.OneData<BoardGameData>().BoardGameConfig;

            foreach (var entity in entities)
            {
                ref var cardComponent = ref entity.GetComponent<CardComponent>();
                cardComponent.RectTransform.localRotation = Quaternion.identity;

                SetMovePositionAnimations(cardComponent.RectTransform , config.SizeCardInTable, entity);
                cardComponent.CardMono.CardOnFace();
            }
        }
        
        public void SetMovePositionAnimations(RectTransform transformObject, Vector3 scale, Entity entity)
        {
            _sequence = DOTween.Sequence();
            var distance = Vector2.Distance(transformObject.anchoredPosition, Vector2.zero);
            var time = distance / 600;
            if (time > 0.8f)
                time = 0.8f;
            _sequence.Append(transformObject.DOAnchorPos(Vector2.zero, time))
                .Join(transformObject.DOScale(scale, time))
                .OnComplete(() => EndMoveCardAnimations(entity));
        }

        public async void EndMoveCardAnimations(Entity entity)
        {
            ActionCardEvent.UpdateValueResourcePlayedCard?.Invoke();
            await Task.Delay(500);

            AnimationsMoveAtDiscardDeckCorotine(entity);
        }

        public void EndRound()
        {
            var entities = _dataWorld.Select<CardComponent>().With<CardTableComponent>().GetEntities();

            foreach (var entity in entities)
            {
                entity.AddComponent(new CardMoveToDiscardComponent());
                entity.RemoveComponent<CardTableComponent>();
                AnimationsMoveAtDiscardDeckAction.AnimationsMoveAtDiscardDeck?.Invoke();
            }
        }

        private async void AnimationsMoveAtDiscardDeckCorotine(Entity entity)
        {
            var sizeCard = _dataWorld.OneData<BoardGameData>().BoardGameConfig.SizeCardInDeck;
            var targetPosition = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CoreHudUIMono.PositionForUseCardPlayer.position;
            var cardComponent = entity.GetComponent<CardComponent>();
            
            var sequence = DOTween.Sequence();
            cardComponent.CardMono.CardOnBack();
            sequence.Append(cardComponent.CardMono.RectTransform.DOLocalRotate(new Vector3(0, 180, 0), 0.2f));
            await sequence.AsyncWaitForCompletion();
            
            await Task.Delay(400);
            
            var distance = Vector2.Distance(cardComponent.RectTransform.position, targetPosition);
            var time = distance / 100;
            if (time > 0.8f)
                time = 0.8f;
            
            sequence.Append(cardComponent.RectTransform.DOMove(targetPosition, time))
                     .Join(cardComponent.RectTransform.DOScale(sizeCard, time))
                     .Join(cardComponent.CardMono.BackCardImage.DOColor(new Color32(255, 255, 255, 0), time / 0.5f));

            await Task.Delay((int)(1000 * time));
            entity.RemoveComponent<CardMoveToDiscardComponent>();
            entity.AddComponent(new CardDiscardComponent());
           /* 
            cardComponent.RectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            cardComponent.RectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            cardComponent.RectTransform.position = targetPosition;
            */
        }
    }
}