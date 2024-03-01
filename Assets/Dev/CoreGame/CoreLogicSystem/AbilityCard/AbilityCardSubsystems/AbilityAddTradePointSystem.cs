using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.UI;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityAddTradePointSystem : IPreInitSystem, IDeactivateSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AbilityCardAction.AddTradePoint += CalculateAddResource;
        }
        
        private void CalculateAddResource()
        {
            var entities = _dataWorld.Select<AbilityCardAddResourceComponent>()
                .GetEntities();

            foreach (var entity in entities)
            {
                AddResource(entity);
            }    
        }

        private void AddResource(Entity entity)
        {
            ref var abilityAddResourceComponent = ref entity.GetComponent<AbilityCardAddResourceComponent>();
            ref var actionData = ref _dataWorld.OneData<ActionCardData>();
            
            actionData.TotalTrade += abilityAddResourceComponent.Count;
            
            entity.RemoveComponent<AbilitySelectElementComponent>();
            entity.RemoveComponent<AbilityCardAddResourceComponent>();
            BoardGameUIAction.UpdateStatsPlayersCurrency?.Invoke();
        }

        public void Deactivate()
        {
            AbilityCardAction.AddTradePoint -= CalculateAddResource;
        }
    }
}