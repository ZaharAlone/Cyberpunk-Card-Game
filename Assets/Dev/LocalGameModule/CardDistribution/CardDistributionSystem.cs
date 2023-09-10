using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using UnityEngine;
using CyberNet.Core.UI;

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
            for (int i = 0; i < eventValue.Count; i++)
            {
                var countPlayerEntities = _dataWorld.Select<CardComponent>()
                           .Where<CardComponent>(card => card.PlayerID == eventValue.TargetPlayerID)
                           .With<CardDrawComponent>()
                           .Count();

                if (countPlayerEntities == 0)
                    GlobalCoreGameAction.SortingDiscardCard?.Invoke(eventValue.TargetPlayerID);

                var playerEntities = _dataWorld.Select<CardComponent>()
                                               .Where<CardComponent>(card => card.PlayerID == eventValue.TargetPlayerID)
                                               .With<CardDrawComponent>()
                                               .GetEntities();

                var id = SortingCard.ChooseNearestCard(playerEntities);
                AddCard(id);
            }
            
            var entity = _dataWorld.NewEntity();
            entity.AddComponent(new WaitDistributionCardHandComponent {
                PlayerID = eventValue.TargetPlayerID,
                CountCard = eventValue.Count
            });
        }

        private void AddCard(int entityId)
        {
            var entity = _dataWorld.GetEntity(entityId);
            ref var cardComponent = ref entity.GetComponent<CardComponent>();

            entity.RemoveComponent<CardDrawComponent>();
            entity.AddComponent(new CardHandComponent());
            entity.AddComponent(new CardDistributionComponent());
            var waitTimeAnim = SortingDeckCardAnimationsAction.GetTimeCardToHand.Invoke(cardComponent.PlayerID);
            waitTimeAnim += 0.175f;
            entity.AddComponent(new WaitAnimationsDrawHandCardComponent { PlayerID = cardComponent.PlayerID, WaitTime = waitTimeAnim });
            cardComponent.CardMono.ShowCard();
        }
    }
}