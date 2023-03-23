using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using UnityEngine;
using BoardGame.Core.UI;

namespace BoardGame.Core
{
    [EcsSystem(typeof(BoardGameModule))]
    public class CardDistributionSystem : IPostRunEventSystem<EventDistributionCard>
    {
        private DataWorld _dataWorld;

        public void PostRunEvent(EventDistributionCard value)
        {
            DistributionCard(value);
        }

        private void DistributionCard(EventDistributionCard eventValue)
        {
            if (eventValue.Target == PlayerEnum.Player1)
            {
                for (int i = 0; i < eventValue.Count; i++)
                {
                    var countPlayerEntities = _dataWorld.Select<CardComponent>()
                               .With<CardPlayer1Component>()
                               .Without<CardDiscardComponent>()
                               .Without<CardHandComponent>()
                               .Count();

                    if (countPlayerEntities == 0)
                        SortingDiscardPlayer();

                    var playerEntities = _dataWorld.Select<CardComponent>()
                                                   .With<CardPlayer1Component>()
                                                   .Without<CardDiscardComponent>()
                                                   .Without<CardHandComponent>()
                                                   .GetEntities();

                    var id = SortingCard.SelectCard(playerEntities);
                    AddCard(id, PlayerEnum.Player1);
                }
            }
            else
            {
                for (int i = 0; i < eventValue.Count; i++)
                {
                    var countEnemyEntities = _dataWorld.Select<CardComponent>()
                              .With<CardPlayer2Component>()
                              .Without<CardDiscardComponent>()
                              .Without<CardHandComponent>()
                              .Count();

                    if (countEnemyEntities == 0)
                        SortingDiscardEnemy();

                    var enemyEntities = _dataWorld.Select<CardComponent>()
                                                  .With<CardPlayer2Component>()
                                                  .Without<CardDiscardComponent>()
                                                  .Without<CardHandComponent>()
                                                  .GetEntities();

                    var id = SortingCard.SelectCard(enemyEntities);
                    AddCard(id, PlayerEnum.Player2);
                }
            }

            _dataWorld.RiseEvent(new EventUpdateHandUI());
            _dataWorld.RiseEvent(new EventUpdateBoardCard());
        }

        private void AddCard(int entityId, PlayerEnum targetPlayer)
        {
            var entity = _dataWorld.GetEntity(entityId);
            var config = _dataWorld.OneData<BoardGameData>().BoardGameConfig;
            entity.AddComponent(new CardHandComponent());

            ref var cardComponent = ref entity.GetComponent<CardComponent>();
            cardComponent.GO.SetActive(true);
            if (targetPlayer == PlayerEnum.Player1)
            {
                cardComponent.Transform.position = config.PlayerHandPosition;
                cardComponent.Transform.localScale = config.NormalSize;
                cardComponent.CardMono.CardOnFace();
            }
            else
            {
                cardComponent.Transform.position = config.PlayerHandPosition;
            }
        }

        private void SortingDiscardPlayer()
        {
            var discardCard = _dataWorld.Select<CardComponent>()
                                        .With<CardPlayer1Component>()
                                        .With<CardDiscardComponent>()
                                        .GetEntities();

            foreach (var entity in discardCard)
                entity.RemoveComponent<CardDiscardComponent>();

            var deckCard = _dataWorld.Select<CardComponent>()
                                     .With<CardPlayer1Component>()
                                     .Without<CardDiscardComponent>()
                                     .Without<CardHandComponent>()
                                     .GetEntities();

            var count = _dataWorld.Select<CardComponent>()
                                  .With<CardPlayer1Component>()
                                  .Without<CardDiscardComponent>()
                                  .Without<CardHandComponent>()
                                  .Count();
            //Rework
            //SortingCard.FirstSorting(count, deckCard);
            _dataWorld.RiseEvent(new EventUpdateStackCard());
        }

        private void SortingDiscardEnemy()
        {
            var discardCard = _dataWorld.Select<CardComponent>()
                                        .With<CardPlayer2Component>()
                                        .With<CardDiscardComponent>()
                                        .GetEntities();

            foreach (var entity in discardCard)
                entity.RemoveComponent<CardDiscardComponent>();

            var deckCard = _dataWorld.Select<CardComponent>()
                                     .With<CardPlayer2Component>()
                                     .Without<CardDiscardComponent>()
                                     .Without<CardHandComponent>()
                                     .GetEntities();

            var count = _dataWorld.Select<CardComponent>()
                                  .With<CardPlayer2Component>()
                                  .Without<CardDiscardComponent>()
                                  .Without<CardHandComponent>()
                                  .Count();
            //Rework
            //SortingCard.FirstSorting(count, deckCard);
            _dataWorld.RiseEvent(new EventUpdateStackCard());
        }
    }
}