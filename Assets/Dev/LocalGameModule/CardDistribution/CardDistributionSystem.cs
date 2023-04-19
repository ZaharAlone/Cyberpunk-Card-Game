using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using UnityEngine;
using BoardGame.Core.UI;

namespace BoardGame.Core
{
    /// <summary>
    /// ������ ����� �������
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
                           .Where<CardComponent>(card => card.Player == eventValue.Target)
                           .With<CardDrawComponent>()
                           .Count();

                if (countPlayerEntities == 0)
                    GlobalCoreGameAction.SortingDiscardCard?.Invoke(eventValue.Target);

                var playerEntities = _dataWorld.Select<CardComponent>()
                                               .Where<CardComponent>(card => card.Player == eventValue.Target)
                                               .With<CardDrawComponent>()
                                               .GetEntities();

                var id = SortingCard.ChooseNearestCard(playerEntities);
                AddCard(id);
            }
            var entity = _dataWorld.NewEntity();
            entity.AddComponent(new WaitDistributionCardHandComponent { Player = eventValue.Target, CountCard = eventValue.Count });
        }

        private void AddCard(int entityId)
        {
            var entity = _dataWorld.GetEntity(entityId);
            var viewData = _dataWorld.OneData<ViewPlayerData>();
            ref var cardComponent = ref entity.GetComponent<CardComponent>();

            entity.RemoveComponent<CardDrawComponent>();
            entity.AddComponent(new CardHandComponent());
            entity.AddComponent(new CardDistributionComponent());
            var waitTimeAnim = WaitCardAnimationsAction.GetTimeCardToHand.Invoke(cardComponent.Player);
            waitTimeAnim += 0.25f;
            entity.AddComponent(new WaitCardAnimationsDrawHandComponent { Player = cardComponent.Player, WaitTime = waitTimeAnim });
            cardComponent.CardMono.ShowCard();
        }
    }
}