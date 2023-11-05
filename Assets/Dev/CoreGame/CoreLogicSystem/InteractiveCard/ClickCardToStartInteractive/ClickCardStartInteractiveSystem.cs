using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.Player;
using CyberNet.Core.UI.CardPopup;
using Input;

namespace CyberNet.Core.InteractiveCard
{
    [EcsSystem(typeof(CoreModule))]
    public class ClickCardStartInteractiveSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            InteractiveActionCard.StartInteractiveCard += DownClickCard;
        }

        private void DownClickCard(string guid)
        {
            var entity = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == guid)
                .SelectFirstEntity();
            
            ref var roundData = ref _dataWorld.OneData<RoundData>();
            if (roundData.PauseInteractive)
                return;
            
            ref var component = ref entity.GetComponent<CardComponent>();
            if (entity.HasComponent<PlayerComponent>())
                return;

            CardPopupAction.ClosePopupCard?.Invoke();
            entity.AddComponent(new CardSelectAbilityComponent());
            
            /*
            var abilityCardConfig = _dataWorld.OneData<CardsConfig>().AbilityCard;
            
            
            
            if (entity.HasComponent<CardHandComponent>() || entity.HasComponent<CardFreeToBuyComponent>())
            {
                ref var inputData = ref _dataWorld.OneData<InputData>();

                entity.AddComponent(new InteractiveMoveComponent
                {
                    StartCardPosition = component.RectTransform.anchoredPosition,
                    StartCardRotation = component.RectTransform.localRotation,
                    StartMousePositions = inputData.MousePosition
                });
            }*/
        }
        
        public void Destroy()
        {
            InteractiveActionCard.StartInteractiveCard -= DownClickCard;
        }
    }
}