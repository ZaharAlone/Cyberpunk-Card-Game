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
                           .Where<CardComponent>(card => card.Player == eventValue.Target)
                           .With<CardDrawComponent>()
                           .Count();

                if (countPlayerEntities == 0)
                {
                    GlobalCoreGameAction.SortingDiscardCardAnimations?.Invoke(eventValue.Target);
                    GlobalCoreGameAction.SortingDiscardCard?.Invoke(eventValue.Target);
                }

                var playerEntities = _dataWorld.Select<CardComponent>()
                                               .Where<CardComponent>(card => card.Player == eventValue.Target)
                                               .With<CardDrawComponent>()
                                               .GetEntities();

                var id = SortingCard.ChooseNearestCard(playerEntities);
                AddCard(id, eventValue.Target);
            }

            _dataWorld.RiseEvent(new EventUpdateHandUI { TargetPlayer = eventValue.Target });
            _dataWorld.RiseEvent(new EventUpdateBoardCard());
        }

        private void AddCard(int entityId, PlayerEnum targetPlayer)
        {
            var entity = _dataWorld.GetEntity(entityId);
            var viewData = _dataWorld.OneData<ViewPlayerData>();
            entity.RemoveComponent<CardDrawComponent>();
            entity.AddComponent(new CardHandComponent());

            ref var cardComponent = ref entity.GetComponent<CardComponent>();
            cardComponent.CardMono.ShowCard();

            if (targetPlayer == viewData.PlayerView)
                cardComponent.CardMono.CardOnFace();
        }
    }
}