using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.UI;

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
            var countCard = _dataWorld.Select<CardComponent>().With<CardTableComponent>().Count();
            var config = _dataWorld.OneData<BoardGameData>().BoardGameConfig;

            var width = (204 + 30) * (countCard - 1);
            var start_point = width / -2;
            
            foreach (var entity in entities)
            {
                ref var cardComponent = ref entity.GetComponent<CardComponent>();
                cardComponent.RectTransform.localRotation = Quaternion.identity;

                var targetPosition = Vector2.zero;
                targetPosition.x = start_point;
                
                SetMovePositionAnimations(cardComponent.RectTransform, targetPosition, config.SizeCardInTable, entity);
                cardComponent.CardMono.CardOnFace();
                start_point += (int)(234 * config.SizeCardInTable.x);
            }
        }
        
        public void SetMovePositionAnimations(RectTransform transformObject, Vector2 targetPosition, Vector3 scale, Entity entity)
        {
            _sequence = DOTween.Sequence();
            var distance = Vector2.Distance(transformObject.anchoredPosition, targetPosition);
            
            var time = distance / 600;
            if (time > 0.8f)
                time = 0.8f;
            
            _sequence.Append(transformObject.DOAnchorPos(targetPosition, time))
                .Join(transformObject.DOScale(scale, time))
                .OnComplete(() => EndMoveCardAnimations(entity));
        }

        public async void EndMoveCardAnimations(Entity entity)
        {
            ActionCardEvent.UpdateValueResourcePlayedCard?.Invoke();
            await Task.Delay(500);

            AnimationsMoveAtDiscardDeckCorotine(entity);
        }
        
        private async void AnimationsMoveAtDiscardDeckCorotine(Entity entity)
        {
            var sizeCard = _dataWorld.OneData<BoardGameData>().BoardGameConfig.SizeCardPlayingDeck;
            var targetPosition = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CoreHudUIMono.PositionForUseCardPlayer.position;
            var cardComponent = entity.GetComponent<CardComponent>();
            
            var distance = Vector2.Distance(cardComponent.RectTransform.position, targetPosition);
            var time = distance / 100;
            if (time > 0.8f)
                time = 0.8f;
            
            var sequence = DOTween.Sequence();
            sequence.Append(cardComponent.RectTransform.DOMove(targetPosition, time))
                .Join(cardComponent.RectTransform.DOScale(sizeCard, time));

            await Task.Delay((int)(1000 * time));
            entity.RemoveComponent<CardMoveToDiscardComponent>();
            entity.AddComponent(new CardDiscardComponent());
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
    }
}