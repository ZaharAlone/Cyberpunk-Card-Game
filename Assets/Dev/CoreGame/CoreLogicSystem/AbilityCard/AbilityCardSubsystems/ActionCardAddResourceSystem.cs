using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.City;
using CyberNet.Core.UI;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class ActionCardAddResourceSystem : IPreInitSystem, IDeactivateSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AbilityCardAction.AddResource += CalculateAddResource;
        }
        
        private void CalculateAddResource()
        {
            var entities = _dataWorld.Select<ActionCardAddResourceComponent>()
                .Without<CardComponentAnimations>()
                .GetEntities();

            foreach (var entity in entities)
            {
                AddResource(entity);
            }    
        }

        private void AddResource(Entity entity)
        {
            ref var abilityAddResourceComponent = ref entity.GetComponent<ActionCardAddResourceComponent>();
            ref var actionData = ref _dataWorld.OneData<ActionCardData>();
            var abilityVFX = _dataWorld.OneData<AbilityCardConfigData>().AbilityCardConfig;
            ref var cardComponent = ref entity.GetComponent<CardComponent>();
            ref var cardsContainer = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CardsContainer;
            
            actionData.TotalTrade += abilityAddResourceComponent.Count;
            ActionCardVisualEffect.CreateEffect(abilityVFX.tradeVFX,cardComponent.RectTransform.position, cardsContainer, abilityAddResourceComponent.Count);
            
            entity.RemoveComponent<ActionCardAddResourceComponent>();
            BoardGameUIAction.UpdateStatsPlayersCurrency?.Invoke();
            CityAction.UpdateCanInteractiveMap?.Invoke();
        }

        public void Deactivate()
        {
            AbilityCardAction.AddResource -= CalculateAddResource;
        }
    }
}