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
            if (eventValue.Target == PlayerEnum.Player)
            {
                for (int i = 0; i < eventValue.Count; i++)
                {
                    var playerEntities = _dataWorld.Select<CardComponent>().With<CardPlayerComponent>().Without<CardDiscardComponent>().Without<CardHandComponent>().GetEntities();
                    var id = SortingCard.SelectCard(playerEntities);
                    AddCard(id);
                }
            }
            else
            {
                for (int i = 0; i < eventValue.Count; i++)
                {
                    var enemyEntities = _dataWorld.Select<CardComponent>().With<CardEnemyComponent>().Without<CardDiscardComponent>().Without<CardHandComponent>().GetEntities();
                    var id = SortingCard.SelectCard(enemyEntities);
                    AddCard(id);
                }
            }

            _dataWorld.RiseEvent(new EventUpdateHandUI());
            _dataWorld.RiseEvent(new EventUpdateBoardCard());
        }
        
        private void AddCard(int entityId)
        {
            var entity = _dataWorld.GetEntity(entityId);
            var config = _dataWorld.OneData<BoardGameData>().BoardGameConfig;
            entity.AddComponent(new CardHandComponent());

            ref var cardComponent = ref entity.GetComponent<CardComponent>();
            cardComponent.Transform.position = config.PlayerHandPosition;
            cardComponent.Transform.localScale = config.NormalSize;
            cardComponent.CardMono.CardOnFace();
        }
    }
}