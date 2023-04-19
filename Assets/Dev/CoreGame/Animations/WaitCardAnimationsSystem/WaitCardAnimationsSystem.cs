using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using DG.Tweening;
using BoardGame.Core.UI;
using System.Threading.Tasks;

namespace BoardGame.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class WaitCardAnimationsSystem : IRunSystem
    {
        private DataWorld _dataWorld;

        public void Run()
        {
            var entitiesSortingDeck = _dataWorld.Select<WaitCardAnimationsSortingDeckComponent>()
                                                .GetEntities();

            foreach (var entity in entitiesSortingDeck)
            {
                ref var waitComponent = ref entity.GetComponent<WaitCardAnimationsSortingDeckComponent>();
                waitComponent.WaitTime -= Time.deltaTime;

                if (waitComponent.WaitTime <= 0)
                    SortingDeckCard(entity);
            }

            var countSortingDeck = _dataWorld.Select<WaitCardAnimationsSortingDeckComponent>()
                                    .Count();

            if (countSortingDeck != 0)
                return;

            var entitiesDrawHand = _dataWorld.Select<WaitCardAnimationsDrawHandComponent>()
                                             .Without<WaitCardAnimationsSortingDeckComponent>()
                                             .GetEntities();
            foreach (var entity in entitiesDrawHand)
            {
                ref var waitComponent = ref entity.GetComponent<WaitCardAnimationsDrawHandComponent>();
                waitComponent.WaitTime -= Time.deltaTime;

                if (entity.HasComponent<CardComponentAnimations>())
                    continue;

                if (waitComponent.WaitTime <= 0)
                    AnimationsDrawToHand(entity);
            }
        }

        private void SortingDeckCard(Entity entity)
        {
            var boardGameConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig;
            var viewData = _dataWorld.OneData<ViewPlayerData>();
            var cardComponent = entity.GetComponent<CardComponent>();
            var ui = _dataWorld.OneData<UIData>().UIMono;
            var targetPositions = Vector3.zero;

            if (viewData.PlayerView == cardComponent.Player)
                targetPositions = ui.DownDeck.localPosition;
            else
                targetPositions = ui.UpDeck.localPosition;

            var animationComponent = new CardComponentAnimations();
            animationComponent.Sequence = DOTween.Sequence();
            animationComponent.Sequence.Append(cardComponent.Transform.DOMove(targetPositions, 0.5f))
                                       .Join(cardComponent.CardMono.BackCardImage.DOColor(new Color32(255, 255, 255, 255), 0.15f))
                                       .Append(cardComponent.CardMono.BackCardImage.DOColor(new Color32(255, 255, 255, 0), 0.15f))
                                       .OnComplete(() => EndSortingCard(entity));

            entity.AddComponent(animationComponent);
            entity.RemoveComponent<WaitCardAnimationsSortingDeckComponent>();
        }

        private void EndSortingCard(Entity entity)
        {
            var cardComponent = entity.GetComponent<CardComponent>();
            cardComponent.CardMono.CardConteinerTransform.rotation = Quaternion.identity;
            var animationComponent = entity.GetComponent<CardComponentAnimations>();
            animationComponent.Sequence.Kill();
            entity.RemoveComponent<CardComponentAnimations>();
        }

        private void AnimationsDrawToHand(Entity entity)
        {
            var boardGameConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig;
            var viewPlayer = _dataWorld.OneData<ViewPlayerData>();
            var cardComponent = entity.GetComponent<CardComponent>();
            var positionsY = -400;

            if (viewPlayer.PlayerView != cardComponent.Player)
                positionsY = 400;

            var animationComponent = new CardComponentAnimations();
            animationComponent.Sequence = DOTween.Sequence();
            animationComponent.Sequence.Append(cardComponent.Transform.DOMove(new Vector3(0, positionsY, 0), 0.4f))
                                       .Join(cardComponent.Transform.DOScale(boardGameConfig.NormalSize, 0.4f))
                                       .Join(cardComponent.CardMono.BackCardImage.DOColor(new Color32(255, 255, 255, 255), 0.1f))
                                       .OnComplete(() => EndDrawHandAnimations(entity));

            entity.AddComponent(animationComponent);
            entity.RemoveComponent<WaitCardAnimationsDrawHandComponent>();
        }

        private void EndDrawHandAnimations(Entity entity)
        {
            var cardComponent = entity.GetComponent<CardComponent>();
            var animationComponent = entity.GetComponent<CardComponentAnimations>();
            animationComponent.Sequence.Kill();
            entity.RemoveComponent<CardComponentAnimations>();
            var viewPlayer = _dataWorld.OneData<ViewPlayerData>();

            if (cardComponent.Player == viewPlayer.PlayerView)
                cardComponent.CardMono.CardOnFace();

            var waitDistributionEntity = _dataWorld.Select<WaitDistributionCardHandComponent>()
                                                   .Where<WaitDistributionCardHandComponent>(wait => wait.Player == cardComponent.Player)
                                                   .SelectFirstEntity();
            ref var waitDistributionComponent = ref waitDistributionEntity.GetComponent<WaitDistributionCardHandComponent>();
            waitDistributionComponent.CurrentDistributionCard++;

            _dataWorld.RiseEvent(new EventCardAnimationsHand { TargetPlayer = cardComponent.Player });
            _dataWorld.RiseEvent(new EventUpdateBoardCard());
        }
    }
}