using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace CyberNet.Core
{
    /// <summary>
    /// Сортировка карт в колоде и старт анимации
    /// </summary>
    [EcsSystem(typeof(LocalGameModule))]
    public class SortingDiscardCardLocalSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            GlobalCoreGameAction.SortingDiscardCard += SortingCardPlayer;
        }

        private void SortingCardPlayer(int playerID)
        {
            var discardCard = _dataWorld.Select<CardComponent>()
                                        .Where<CardComponent>(card => card.PlayerID == playerID)
                                        .With<CardDiscardComponent>()
                                        .GetEntities();
            var countCard = _dataWorld.Select<CardComponent>()
                                        .Where<CardComponent>(card => card.PlayerID == playerID)
                                        .With<CardDiscardComponent>()
                                        .Count();

            var sorting = SortingCard.Sorting(countCard);
            var index = 0;

            foreach (var entity in discardCard)
            {
                entity.RemoveComponent<CardDiscardComponent>();
                entity.AddComponent(new CardDrawComponent());
                var waitTimeAnim = SortingDeckCardAnimationsAction.GetTimeSortingDeck.Invoke(playerID);
                waitTimeAnim += 0.07f;
                entity.AddComponent(new WaitEndAnimationsToStartMoveHandComponent { PlayerID = playerID, WaitTime = waitTimeAnim });
                ref var cardIndexComponent = ref entity.GetComponent<CardSortingIndexComponent>();
                cardIndexComponent.Index = sorting[index];
                index++;
            }
        }

        public void Destroy()
        {
            GlobalCoreGameAction.SortingDiscardCard -= SortingCardPlayer;
        }
    }
}