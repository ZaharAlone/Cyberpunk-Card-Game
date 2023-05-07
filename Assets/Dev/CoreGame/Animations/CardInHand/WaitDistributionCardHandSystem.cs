using DG.Tweening;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System.Threading.Tasks;

namespace CyberNet.Core
{
    /// <summary>
    /// Ждет пока игрок получит все карты на руку, чтобы отсортировать их
    /// </summary>
    [EcsSystem(typeof(CoreModule))]
    public class WaitDistributionCardHandSystem : IRunSystem
    {
        private DataWorld _dataWorld;

        public void Run()
        {
            var entities = _dataWorld.Select<WaitDistributionCardHandComponent>().GetEntities();

            foreach (var entity in entities)
            {
                ref var component = ref entity.GetComponent<WaitDistributionCardHandComponent>();
                if (component.CountCard == component.CurrentDistributionCard)
                    EndDistributionCardHand(entity, component.Player);
            }
        }

        public void EndDistributionCardHand(Entity entity, PlayerEnum player)
        {
            entity.Destroy();
            var entitiesCards = _dataWorld.Select<CardComponent>()
                             .Where<CardComponent>(card => card.Player == player)
                             .With<CardDistributionComponent>()
                             .GetEntities();
            foreach (var entityCard in entitiesCards)
                entityCard.RemoveComponent<CardDistributionComponent>();
        }
    }
}