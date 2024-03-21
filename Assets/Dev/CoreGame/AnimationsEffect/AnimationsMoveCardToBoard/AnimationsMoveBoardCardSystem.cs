using System.Collections.Generic;
using System.Linq;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.UI;
using CyberNet.Global.Sound;
using ModulesFramework.Data.Enumerators;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class AnimationsMoveBoardCardSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;
        private Sequence _sequence;
        
        public void PreInit()
        {
            AnimationsMoveBoardCardAction.AnimationsMoveBoardCard += AnimationsMoveBoardCard;
        }

        private void AnimationsMoveBoardCard()
        {
            var cardMoveToTableQuery = _dataWorld.Select<CardComponent>()
                .With<CardStartMoveToTableComponent>();
            var entities = cardMoveToTableQuery.GetEntities();
            var config = _dataWorld.OneData<BoardGameData>().BoardGameConfig;

            var width = (204 + 30) * (cardMoveToTableQuery.Count() - 1);
            var start_point = width / -2;

            var sortCard = SortingCardInHand(entities);
            foreach (var entity in sortCard)
            {
                ref var cardComponent = ref entity.GetComponent<CardComponent>();
                cardComponent.RectTransform.localRotation = Quaternion.identity;

                var targetPosition = Vector2.zero;
                targetPosition.x = start_point;
                
                SetMovePositionAnimations(cardComponent.RectTransform, targetPosition, config.SizeCardInTable, entity);
                cardComponent.CardMono.CardOnFace();
                start_point += (int)(234 * config.SizeCardInTable.x);
            }

            foreach (var entity in entities)
            {
                entity.RemoveComponent<CardStartMoveToTableComponent>();
                entity.AddComponent(new CardMoveToTableComponent());
            }
        }

        private List<Entity> SortingCardInHand(EntitiesEnumerable entities)
        {
            var tempList = new List<Entity>();

            foreach (var entity in entities)
            {
                tempList.Add(entity);
            }

            var sortCards = tempList.OrderBy(item => item.GetComponent<CardSortingIndexComponent>().Index);
            var sortingCardList = new List<Entity>();
            
            foreach (var entity in sortCards)
            {
                sortingCardList.Add(entity);
            }
            
            return sortingCardList;
        } 
        
        public void SetMovePositionAnimations(RectTransform transformObject, Vector2 targetPosition, Vector3 scale, Entity entity)
        {
            _sequence = DOTween.Sequence();
            var distance = Vector2.Distance(transformObject.anchoredPosition, targetPosition);
            
            var time = distance / 800;
            if (time > 0.5f)
                time = 0.5f;
            
            _sequence.Append(transformObject.DOAnchorPos(targetPosition, time))
                .Join(transformObject.DOScale(scale, time))
                .OnComplete(() => EndMoveCardAnimations(entity));
        }

        public async void EndMoveCardAnimations(Entity entity)
        {
            AbilityCardAction.UpdateValueResourcePlayedCard?.Invoke();
            await Task.Delay(150);
            
            AnimationsMoveAtEndPlayingCardDeck(entity);
        }
        
        private void AnimationsMoveAtEndPlayingCardDeck(Entity entity)
        {
            var sizeCard = _dataWorld.OneData<BoardGameData>().BoardGameConfig.SizeCardPlayingDeck;
            var targetPosition = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CoreHudUIMono.PositionForUseCardPlayer.position;
            var cardComponent = entity.GetComponent<CardComponent>();
            
            var distance = Vector2.Distance(cardComponent.RectTransform.position, targetPosition);
            var time = distance / 500;
            if (time > 0.6f)
                time = 0.6f;
            
            var sequence = DOTween.Sequence();
            sequence.Append(cardComponent.RectTransform.DOMove(targetPosition, time))
                .Join(cardComponent.RectTransform.DOScale(sizeCard, time))
                .OnComplete(() => EndAnimation(entity));
        }

        private void EndAnimation(Entity entity)
        {
            entity.RemoveComponent<CardMoveToTableComponent>();
            entity.AddComponent(new CardInTableComponent());
        }

        public void Destroy()
        {
            AnimationsMoveBoardCardAction.AnimationsMoveBoardCard -= AnimationsMoveBoardCard;
        }
    }
}