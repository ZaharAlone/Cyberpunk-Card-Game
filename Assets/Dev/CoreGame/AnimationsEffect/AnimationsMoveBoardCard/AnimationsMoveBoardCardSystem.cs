using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.Ability;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class AnimationsMoveBoardCardSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

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
            
            Debug.LogError($"Animations Move Board Card {countCard.ToString()}");
            
            foreach (var entity in entities)
            {
                Debug.LogError("Move Card");
                ref var cardComponent = ref entity.GetComponent<CardComponent>();
                cardComponent.Transform.rotation = Quaternion.identity;
                var pos = config.PlayerCardPositionInPlay;
                pos.x = start_point;

                cardComponent.CardMono.SetMovePositionAnimations(pos, config.SizeCardInTable);
                cardComponent.CardMono.CardOnFace();

                start_point += (int)(234 * config.SizeCardInTable.x);
            }
            
            Debug.LogError("MoveAnimations Event");
            //To-do переписать на окончание анимации движения в дальнейшем
            AbilityEvent.UpdateValueResourcePlayedCard?.Invoke();
        }
    }
}