using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using DG.Tweening;
using UnityEngine;
using BoardGame.Core.UI;

namespace BoardGame.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class SortingDiscardCardAnimationsSystem : IInitSystem
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            GlobalCoreGameAction.SortingDiscardCardAnimations += AnimationsSortingCard;
        }

        private void AnimationsSortingCard(PlayerEnum player)
        {
            var discardCard = _dataWorld.Select<CardComponent>()
                                        .Where<CardComponent>(card => card.Player == player)
                                        .With<CardDiscardComponent>()
                                        .GetEntities();
            var countCard = _dataWorld.Select<CardComponent>()
                                        .Where<CardComponent>(card => card.Player == player)
                                        .With<CardDiscardComponent>()
                                        .Count();

            var ui = _dataWorld.OneData<UIData>().UIMono;
            var viewData = _dataWorld.OneData<ViewPlayerData>();
            var targetPositions = Vector3.zero;
            if (viewData.PlayerView == player)
                targetPositions = ui.DownDeck.localPosition;
            else
                targetPositions = ui.UpDiscard.localPosition;

            var counter = 0f;
            foreach (var entity in discardCard)
            {
                ref var cardComponent = ref entity.GetComponent<CardComponent>();
                cardComponent.CardMono.BackCardImage.color = new Color32(1, 1, 1, 1);

                var sequence = DOTween.Sequence();
                sequence.PrependInterval(0.05f * counter)
                        .Append(cardComponent.Transform.DOMove(targetPositions, 0.5f))
                        .PrependInterval(0.4f)
                        .Join(cardComponent.CardMono.BackCardImage.DOColor(new Color32(1, 1, 1, 0), 0.1f));
                counter++;
            }
        }
    }
}