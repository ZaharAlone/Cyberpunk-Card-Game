using CyberNet.Core.InteractiveCard;
using CyberNet.Core.Player;
using CyberNet.Global;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems.Events;
using UnityEngine;

namespace CyberNet.Core
{
    /// <summary>
    /// Выдаем карты игрокам
    /// </summary>
    [EcsSystem(typeof(LocalGameModule))]
    public class CardDistributionSystem : IPostRunEventSystem<EventDistributionCard>
    {
        private DataWorld _dataWorld;

        public void PostRunEvent(EventDistributionCard value)
        {
            DistributionCard(value);
        }

        private void DistributionCard(EventDistributionCard eventValue)
        {
            var targetPlayerEntity = _dataWorld.Select<PlayerComponent>().
                Where<PlayerComponent>(player => player.PlayerID == eventValue.TargetPlayerID)
                .SelectFirstEntity();
            var playerComponent = targetPlayerEntity.GetComponent<PlayerComponent>();
            
            var isShowView = playerComponent.playerOrAI == PlayerOrAI.Player;
            if (!targetPlayerEntity.HasComponent<CurrentPlayerComponent>())
                isShowView = false;
            
            for (int i = 0; i < eventValue.Count; i++)
            {
                var countCardsPlayerEntities = _dataWorld.Select<CardComponent>()
                           .Where<CardComponent>(card => card.PlayerID == eventValue.TargetPlayerID)
                           .With<CardDrawComponent>()
                           .Count();

                if (countCardsPlayerEntities == 0)
                    GlobalCoreGameAction.SortingDiscardCard?.Invoke(eventValue.TargetPlayerID);

                var cardEntities = _dataWorld.Select<CardComponent>()
                                               .Where<CardComponent>(card => card.PlayerID == eventValue.TargetPlayerID)
                                               .With<CardDrawComponent>()
                                               .GetEntities();

                var id = SortingCard.ChooseNearestCard(cardEntities);
                
                if (id == 0)
                    return;
                AddCard(id, isShowView);
            }

            if (isShowView)
            {
                var entity = _dataWorld.NewEntity();
                entity.AddComponent(new WaitDistributionCardHandComponent {
                    PlayerID = eventValue.TargetPlayerID,
                    CountCard = eventValue.Count
                });   
            }
        }

        private void AddCard(int entityId, bool isShowView)
        {
            var entity = _dataWorld.GetEntity(entityId);
            ref var cardComponent = ref entity.GetComponent<CardComponent>();

            entity.RemoveComponent<CardDrawComponent>();
            entity.AddComponent(new CardHandComponent());
            entity.AddComponent(new CardDistributionComponent());

            if (isShowView)
            {
                var waitTimeAnim = SortingDeckCardAnimationsAction.GetTimeCardToHand.Invoke(cardComponent.PlayerID);
                waitTimeAnim += 0.175f;
                entity.AddComponent(new WaitAnimationsDrawHandCardComponent { PlayerID = cardComponent.PlayerID, WaitTime = waitTimeAnim });
                cardComponent.CardMono.ShowCard();   
            }
        }
    }
}