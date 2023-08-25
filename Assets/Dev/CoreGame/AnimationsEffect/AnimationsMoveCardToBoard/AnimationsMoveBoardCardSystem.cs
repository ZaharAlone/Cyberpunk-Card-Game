using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using System.Threading.Tasks;
using DG.Tweening;
using CyberNet.Core.ActionCard;

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
            entity.AddComponent(new CardMoveToDiscardComponent());
            entity.RemoveComponent<CardTableComponent>();
            AnimationsMoveAtDiscardDeckAction.AnimationsMoveAtDiscardDeck?.Invoke();
        }
    }
}