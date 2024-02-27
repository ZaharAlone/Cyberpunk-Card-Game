using CyberNet.Core.UI;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;

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
                    EndDistributionCardHand(entity, component.PlayerID);
            }
        }

        public void EndDistributionCardHand(Entity entity, int playerID)
        {
            entity.Destroy();
            var entitiesCards = _dataWorld.Select<CardComponent>()
                             .Where<CardComponent>(card => card.PlayerID == playerID)
                             .With<CardDistributionComponent>()
                             .GetEntities();
            
            foreach (var entityCard in entitiesCards)
                entityCard.RemoveComponent<CardDistributionComponent>();
            
            CardDistributionAction.EndDistributionCard?.Invoke();
        }
    }
}