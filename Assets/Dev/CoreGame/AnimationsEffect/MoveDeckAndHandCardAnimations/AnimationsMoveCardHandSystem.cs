using CyberNet.Core.InteractiveCard;
using CyberNet.Core.Sound;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using DG.Tweening;
using CyberNet.Core.UI;
using CyberNet.Global.Sound;

namespace CyberNet.Core
{
    /// <summary>
    /// Система отвечает за движение карты в руку, сначала проверяет осталась ли анимация движения карты в колоду
    /// </summary>
    [EcsSystem(typeof(CoreModule))]
    public class AnimationsMoveCardHandSystem : IRunSystem
    {
        private DataWorld _dataWorld;

        public void Run()
        {
            CheckStartAnimationsMoveCardHand();
        }
        
        private void CheckStartAnimationsMoveCardHand()
        {
            var countSortingDeck = _dataWorld.Select<WaitEndAnimationsToStartMoveHandComponent>()
                .Count();

            if (countSortingDeck != 0)
                return;
            
            var entitiesDrawHand = _dataWorld.Select<WaitAnimationsDrawHandCardComponent>()
                .Without<WaitEndAnimationsToStartMoveHandComponent>()
                .GetEntities();
            
            foreach (var entity in entitiesDrawHand)
            {
                ref var waitComponent = ref entity.GetComponent<WaitAnimationsDrawHandCardComponent>();
                waitComponent.WaitTime -= Time.deltaTime;

                if (entity.HasComponent<CardComponentAnimations>())
                    continue;

                if (waitComponent.WaitTime <= 0)
                    AnimationsDrawToHand(entity);
            }
        }

        //Анимация движения карт в руку
        private void AnimationsDrawToHand(Entity entity)
        {
            var boardGameConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig;
            var cardComponent = entity.GetComponent<CardComponent>();
            var positionsY = -400;

            var animationComponent = new CardComponentAnimations();
            animationComponent.Sequence = DOTween.Sequence();
            animationComponent.Sequence.Append(cardComponent.RectTransform.DOAnchorPos(new Vector2(0, positionsY), 0.25f))
                                       .Join(cardComponent.RectTransform.DOScale(boardGameConfig.NormalSize, 0.25f))
                                       .Join(cardComponent.CardMono.BackCardImage.DOColor(new Color32(255, 255, 255, 255), 0.07f))
                                       .OnComplete(() => EndDrawHandAnimations(entity));

            entity.AddComponent(animationComponent);
            entity.RemoveComponent<WaitAnimationsDrawHandCardComponent>();
            
            SoundAction.PlaySound?.Invoke(_dataWorld.OneData<SoundData>().Sound.GetCardInHand);
        }
        
        private void EndDrawHandAnimations(Entity entity)
        {
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;
            var cardComponent = entity.GetComponent<CardComponent>();
            var animationComponent = entity.GetComponent<CardComponentAnimations>();

            animationComponent.Sequence.Kill();
            entity.RemoveComponent<CardComponentAnimations>();

            if (cardComponent.PlayerID == currentPlayerID)
                cardComponent.CardMono.CardOnFace();

            var waitDistributionEntity = _dataWorld.Select<WaitDistributionCardHandComponent>()
                                                   .Where<WaitDistributionCardHandComponent>(wait => wait.PlayerID == cardComponent.PlayerID)
                                                   .SelectFirstEntity();
            
            ref var waitDistributionComponent = ref waitDistributionEntity.GetComponent<WaitDistributionCardHandComponent>();
            waitDistributionComponent.CurrentDistributionCard++;

            CardAnimationsHandAction.AnimationsFanCardInHand?.Invoke();
            _dataWorld.RiseEvent(new EventUpdateBoardCard());
            BoardGameUIAction.UpdateCountCardInHand?.Invoke();
        }
    }
}