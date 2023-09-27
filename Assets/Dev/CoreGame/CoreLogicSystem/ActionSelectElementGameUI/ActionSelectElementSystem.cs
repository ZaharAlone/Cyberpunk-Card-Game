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
    public class ActionSelectElementSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            ActionSelectElementAction.SelectCard += SelectCard;
            ActionSelectElementAction.SelectEnemyPlayer += SelectEnemyPlayer;
            ActionSelectElementAction.CloseWindowAbilitySelectCard += CloseWindow;
            ActionSelectElementAction.OpenSelectAbilityCard += OpenWindow;
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
            
            var entity = _dataWorld.Select<CardComponent>().With<ActionSelectElementComponent>().SelectFirstEntity();
            ref var actionSelectCardComponent = ref entity.GetComponent<ActionSelectElementComponent>();
            ref var uiActionSelectCard = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.actionSelectElementUIMono;
            ref var actionConfig = ref _dataWorld.OneData<ActionCardConfigData>().ActionCardViewConfig;
            
            actionConfig.TryGetValue(actionSelectCardComponent.AbilityCard.AbilityType.ToString(), out var actionVisualConfig);
            
            uiActionSelectCard.SetView(actionVisualConfig.HeaderLoc, actionVisualConfig.DescrLoc);
            var isEnableCancelButton = true;
            
            uiActionSelectCard.OpenWindow(isEnableCancelButton);
        }

        private void SelectEnemyPlayer(AbilityType abilityType)
        {
            //One data CardsConfig
        }
        
        private void CloseWindow()
        {
            ref var uiActionSelectCard = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.actionSelectElementUIMono;
            uiActionSelectCard.CloseWindow();
        }

        private void SelectCard()
        {
            
        }
    }
}