using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
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
            var countCard = _dataWorld.Select<CardComponent>().With<CardTableComponent>().Count();
            var entities = _dataWorld.Select<CardComponent>().With<CardTableComponent>().GetEntities();
            var config = _dataWorld.OneData<BoardGameData>().BoardGameConfig;

            var width = (204 + 30) * (countCard - 1);
            var start_point = width / -2;
            
            foreach (var entity in entities)
            {
                ref var cardComponent = ref entity.GetComponent<CardComponent>();
                cardComponent.Transform.rotation = Quaternion.identity;
                var pos = config.PlayerCardPositionInPlay;
                pos.x = start_point;

                SetMovePositionAnimations(cardComponent.Transform ,pos, config.SizeCardInTable);
                cardComponent.CardMono.CardOnFace();

                start_point += (int)(234 * config.SizeCardInTable.x);
            }
        }
        
        public void SetMovePositionAnimations(Transform transformObject, Vector3 positions, Vector3 scale)
        {
            _sequence = DOTween.Sequence();
            var distance = Vector3.Distance(transformObject.position, positions);
            var time = distance / 600;
            if (time > 0.8f)
                time = 0.8f;
            _sequence.Append(transformObject.DOMove(positions, time))
                .Join(transformObject.DOScale(scale, time))
                .OnComplete(() => EndMoveCardAnimations());
        }

        public void EndMoveCardAnimations()
        {
            ActionCardEvent.UpdateValueResourcePlayedCard?.Invoke();
        }
    }
}