using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace BoardGame.Core
{
    [EcsSystem(typeof(LocalGameModule))]
    public class SortingDiscardCardLocalSystem : IInitSystem
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            GlobalCoreGameAction.SortingDiscardCard += SortingCardPlayer;
        }

        private void SortingCardPlayer(PlayerEnum player)
        {
            var discardCard = _dataWorld.Select<CardComponent>()
                                        .Where<CardComponent>(card => card.Player == player)
                                        .With<CardDiscardComponent>()
                                        .GetEntities();
            var countCard = _dataWorld.Select<CardComponent>()
                                        .Where<CardComponent>(card => card.Player == player)
                                        .With<CardDiscardComponent>()
                                        .Count();

            var sorting = SortingCard.Sorting(countCard);
            var index = 0;

            foreach (var entity in discardCard)
            {
                entity.RemoveComponent<CardDiscardComponent>();
                entity.AddComponent(new CardDrawComponent());
                var waitTimeAnim = WaitCardAnimationsAction.GetTimeSortingDeck.Invoke(player);
                waitTimeAnim += 0.1f;
                entity.AddComponent(new WaitCardAnimationsSortingDeckComponent { Player = player, WaitTime = waitTimeAnim });
                ref var cardIndexComponent = ref entity.GetComponent<CardSortingIndexComponent>();
                cardIndexComponent.Index = sorting[index];
                index++;
            }
        }
    }
}