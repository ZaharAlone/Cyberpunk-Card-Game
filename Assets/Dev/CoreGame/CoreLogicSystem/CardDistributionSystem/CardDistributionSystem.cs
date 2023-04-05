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
    [EcsSystem(typeof(CoreModule))]
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
                           .Without<CardDiscardComponent>()
                           .Without<CardHandComponent>()
                           .Count();

                if (countPlayerEntities == 0)
                    GlobalCoreGameAction.SortingDiscardCard?.Invoke(eventValue.Target);

                var player1Entities = _dataWorld.Select<CardComponent>()
                                               .Where<CardComponent>(card => card.Player == eventValue.Target)
                                               .Without<CardDiscardComponent>()
                                               .Without<CardHandComponent>()
                                               .GetEntities();

                var id = SortingCard.ChooseNearestCard(player1Entities);
                AddCard(id, eventValue.Target);
            }

            _dataWorld.RiseEvent(new EventUpdateHandUI { TargetPlayer = eventValue.Target });
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
    }
}