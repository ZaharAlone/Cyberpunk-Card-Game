using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.UI;

namespace CyberNet.Core.ActionCard
{
    /// <summary>
    /// Визуал выбора карты для action карты
    /// </summary>
    [EcsSystem(typeof(CoreModule))]
    public class ActionSelectCardSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            ActionSelectCardAction.SelectCard += SelectCard;
            ActionSelectCardAction.CloseWindowAbilitySelectCard += CloseWindow;
            ActionSelectCardAction.OpenSelectAbilityCard += OpenWindow;
        }
        
        private void OpenWindow()
        {
            var entity = _dataWorld.Select<CardComponent>().With<ActionSelectCardComponent>().SelectFirstEntity();
            ref var actionSelectCardComponent = ref entity.GetComponent<ActionSelectCardComponent>();
            ref var uiActionSelectCard = ref _dataWorld.OneData<UIData>().UIMono.actionSelectCardUIMono;
            ref var actionConfig = ref _dataWorld.OneData<ActionCardConfigData>().ActionCardViewConfig;
            actionConfig.TryGetValue(actionSelectCardComponent.AbilityCard.AbilityType.ToString(), out var actionVisualConfig);
            
            uiActionSelectCard.SetView(actionVisualConfig.HeaderLoc, actionVisualConfig.DescrLoc);
            var isEnableCancelButton = true;

            if (actionSelectCardComponent.AbilityCard.AbilityType == AbilityType.DiscardCardEnemy)
                isEnableCancelButton = false;
            uiActionSelectCard.OpenWindow(isEnableCancelButton);
        }

        private void CloseWindow()
        {
            ref var uiActionSelectCard = ref _dataWorld.OneData<UIData>().UIMono.actionSelectCardUIMono;
            uiActionSelectCard.CloseWindow();
        }
        
        private void SelectCard()
        {
            
        }
    }
}