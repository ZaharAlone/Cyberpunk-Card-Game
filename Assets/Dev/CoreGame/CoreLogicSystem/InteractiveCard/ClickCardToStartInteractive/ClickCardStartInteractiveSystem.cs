using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.Player;
using CyberNet.Core.UI.CorePopup;

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

            CoreElementInfoPopupAction.ClosePopupCard?.Invoke();
            entity.AddComponent(new CardSelectAbilityComponent());
            
            
            /* Смотрим есть ли выбор способности, если есть выбираем одну из них.
             * Если нет играем по правилам конфига - стрелка/либо на стол.
             * Если выбор то после выбора:
             * 1) Если абилка играется на стол - играем её сразу.
             * 2) Если стрелкой - то стрелка активируется сразу
             */
            
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