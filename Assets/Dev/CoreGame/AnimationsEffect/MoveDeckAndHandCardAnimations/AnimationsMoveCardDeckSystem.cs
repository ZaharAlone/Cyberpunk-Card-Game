using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using DG.Tweening;
using CyberNet.Core.UI;

namespace CyberNet.Core
{
    /// <summary>
    /// Система выполняет движение карт в колоду, к примеру из сброса
    /// </summary>
    [EcsSystem(typeof(CoreModule))]
    public class AnimationsMoveCardDeckSystem : IRunSystem
    {
        private DataWorld _dataWorld;

        public void Run()
        {
            var entitiesStartMoveToDeck = _dataWorld.Select<WaitEndAnimationsToStartMoveHandComponent>()
                .GetEntities();

            foreach (var entity in entitiesStartMoveToDeck)
            {
                ref var waitComponent = ref entity.GetComponent<WaitEndAnimationsToStartMoveHandComponent>();
                waitComponent.WaitTime -= Time.deltaTime;

                if (waitComponent.WaitTime <= 0)
                    AnimationsMoveCardToHand(entity);
            }
        }

        private void AnimationsMoveCardToHand(Entity entity)
        {
            var viewData = _dataWorld.OneData<ViewPlayerData>();
            var cardComponent = entity.GetComponent<CardComponent>();
            var ui = _dataWorld.OneData<CoreUIData>().BoardGameUIMono;
            var targetPositions = Vector3.zero;

            //TODO: старый код
            if (viewData.PlayerView == cardComponent.Player)
                targetPositions = ui.CoreHudUIMono.DownDeck.localPosition;
            //else
            //    targetPositions = ui.CoreHudUIMono.UpDeck.localPosition;

            var animationComponent = new CardComponentAnimations();
            animationComponent.Sequence = DOTween.Sequence();
            animationComponent.Sequence.Append(cardComponent.RectTransform.DOMove(targetPositions, 0.5f))
                .Join(cardComponent.CardMono.BackCardImage.DOColor(new Color32(255, 255, 255, 255), 0.15f))
                .Append(cardComponent.CardMono.BackCardImage.DOColor(new Color32(255, 255, 255, 0), 0.15f))
                .OnComplete(() => EndAnimationsMoveCardToHand(entity));

            entity.AddComponent(animationComponent);
            entity.RemoveComponent<WaitEndAnimationsToStartMoveHandComponent>();
        }
        private void EndAnimationsMoveCardToHand(Entity entity)
        {
            var cardComponent = entity.GetComponent<CardComponent>();
            cardComponent.CardMono.RectTransform.rotation = Quaternion.identity;
            var animationComponent = entity.GetComponent<CardComponentAnimations>();
            animationComponent.Sequence.Kill();
            entity.RemoveComponent<CardComponentAnimations>();
        }
    }
}