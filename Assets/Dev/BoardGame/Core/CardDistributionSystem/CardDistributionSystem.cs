using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using BoardGame.Core.UI;

namespace BoardGame.Core
{
    [EcsSystem(typeof(BoardGameModule))]
    public class CardDistributionSystem : IPostRunSystem
    {
        private DataWorld _dataWorld;

        public void PostRun()
        {
            var countEvent = _dataWorld.Select<EventDistributionCard>().Count();

            if (countEvent > 0)
                DistributionCard();                
        }

        private void DistributionCard()
        {
            _dataWorld.TrySelectFirst<EventDistributionCard>(out var eventValue);

            if (eventValue.Target == PlayerEnum.Player)
            {
                for (int i = 0; i < eventValue.Count; i++)
                {
                    var playerEntities = _dataWorld.Select<CardComponent>().With<CardPlayerComponent>().Without<CardInDropComponent>().Without<CardInHandComponent>().GetEntities();
                    var id = SortingCard.SelectCard(playerEntities);
                    AddCard(id);
                }
            }
            else
            {
                for (int i = 0; i < eventValue.Count; i++)
                {
                    var enemyEntities = _dataWorld.Select<CardComponent>().With<CardEnemyComponent>().Without<CardInDropComponent>().Without<CardInHandComponent>().GetEntities();
                    var id = SortingCard.SelectCard(enemyEntities);
                    AddCard(id);
                }
            }

            _dataWorld.CreateOneFrame().AddComponent(new EventUpdateHandUI());
        }
        
        private void AddCard(int entityId)
        {
            var entity = _dataWorld.GetEntity(entityId);
            entity.AddComponent(new CardInHandComponent());
            var pos = _dataWorld.OneData<BoardGameData>().BoardGameConfig.PlayerHandPosition;

            ref var cardComponent = ref entity.GetComponent<CardComponent>();
            cardComponent.Transform.position = pos;
            cardComponent.CardMono.CardOnFace();
        }
    }
}