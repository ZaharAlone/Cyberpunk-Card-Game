using CyberNet.Core.Player;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.UI;
using CyberNet.Global;

namespace CyberNet.Core.AbilityCard
{
    /// <summary>
    /// Визуал выбора карты для ability карты
    /// </summary>
    [EcsSystem(typeof(CoreModule))]
    public class AbilitySelectCardSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AbilitySelectElementAction.ConfimSelect += SelectCard;
            AbilitySelectElementAction.OpenSelectAbilityCard += OpenWindow;
        }

        private void OpenWindow()
        {
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player => player.PlayerID == currentPlayerID)
                .SelectFirstEntity();

            ref var playerComponent = ref playerEntity.GetComponent<PlayerComponent>();
            
            if (playerComponent.playerTypeEnum != PlayerTypeEnum.Player)
                return;
            
            var entity = _dataWorld.Select<CardComponent>().With<AbilitySelectElementComponent>().SelectFirstEntity();
            ref var actionSelectCardComponent = ref entity.GetComponent<AbilitySelectElementComponent>();
            ref var uiActionSelectCard = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.AbilitySelectElementUIMono;
            ref var abilityConfig = ref _dataWorld.OneData<CardsConfig>().AbilityCard;
            
            abilityConfig.TryGetValue(actionSelectCardComponent.AbilityCard.AbilityType.ToString(), out var actionVisualConfig);
            
            uiActionSelectCard.SetView(actionVisualConfig.SelectFrameHeader, actionVisualConfig.SelectFrameDescr);
            var isEnableCancelButton = true;
            
            uiActionSelectCard.OpenWindow(isEnableCancelButton);
        }

        private void SelectCard()
        {
            
        }
    }
}