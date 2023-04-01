using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using System.Collections.Generic;
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

        private void SortingCardPlayer(PlayerEnum targetPlayer)
        {
            if (targetPlayer == PlayerEnum.Player1)
                SortingDiscardPlayer1();
            else
                SortingDiscardPlayer2();
        }

        private void SortingDiscardPlayer1()
        {
            var discardCard = _dataWorld.Select<CardComponent>()
                                        .With<CardPlayer1Component>()
                                        .With<CardDiscardComponent>()
                                        .GetEntities();
            var countCard = _dataWorld.Select<CardComponent>()
                                        .With<CardPlayer1Component>()
                                        .With<CardDiscardComponent>()
                                        .Count();

            var sorting = SortingCard.Sorting(countCard);
            var index = 0;
            foreach (var entity in discardCard)
            {
                entity.RemoveComponent<CardDiscardComponent>();
                ref var cardIndexComponent = ref entity.GetComponent<CardSortingIndexComponent>();
                cardIndexComponent.Index = sorting[index];
                index++;
            }

            _dataWorld.RiseEvent(new EventUpdateDeckCard());
        }

        private void SortingDiscardPlayer2()
        {
            var discardCard = _dataWorld.Select<CardComponent>()
                                        .With<CardPlayer2Component>()
                                        .With<CardDiscardComponent>()
                                        .GetEntities();
            var countCard = _dataWorld.Select<CardComponent>()
                                        .With<CardPlayer2Component>()
                                        .With<CardDiscardComponent>()
                                        .Count();

            var sorting = SortingCard.Sorting(countCard);
            var index = 0;
            foreach (var entity in discardCard)
            {
                entity.RemoveComponent<CardDiscardComponent>();
                ref var cardIndexComponent = ref entity.GetComponent<CardSortingIndexComponent>();
                cardIndexComponent.Index = sorting[index];
                index++;
            }

            _dataWorld.RiseEvent(new EventUpdateDeckCard());
        }
    }
}