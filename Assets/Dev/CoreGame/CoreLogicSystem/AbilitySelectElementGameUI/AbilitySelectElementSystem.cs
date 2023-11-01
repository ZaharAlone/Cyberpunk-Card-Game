using CyberNet.Core.Player;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.UI;
using CyberNet.Global;
using UnityEngine;

namespace CyberNet.Core.AbilityCard
{
    /// <summary>
    /// Визуал выбора карты для ability карты
    /// </summary>
    [EcsSystem(typeof(CoreModule))]
    public class AbilitySelectElementSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;
        private bool _isSubscription;
        
        public void PreInit()
        {
            AbilitySelectElementAction.OpenSelectAbilityCard += OpenWindow;
            AbilitySelectElementAction.SelectElement += SelectElement;
        }

        private void OpenWindow(AbilityType abilityType, int indexDescr, bool basePositionFrame = true)
        {
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();

            ref var playerComponent = ref playerEntity.GetComponent<PlayerComponent>();
            if (playerComponent.playerTypeEnum != PlayerTypeEnum.Player)
                return;
            
            ref var uiActionSelectCard = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.AbilitySelectElementUIMono;
            ref var abilityConfig = ref _dataWorld.OneData<CardsConfig>().AbilityCard;
            
            abilityConfig.TryGetValue(abilityType.ToString(), out var actionVisualConfig);

            if (indexDescr == 0)
            {
                uiActionSelectCard.SetView(actionVisualConfig.SelectFrameHeader, actionVisualConfig.SelectFrameDescr);   
            }
            else
            {
                uiActionSelectCard.SetView(actionVisualConfig.SelectFrameHeader, actionVisualConfig.SelectFrameDescr_2);
            }
            
            uiActionSelectCard.OpenWindow(true, basePositionFrame);
        }
        
        private void SelectElement(string textButton)
        {
            ref var uiActionSelectCard = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.AbilitySelectElementUIMono;
            uiActionSelectCard.SetTextButtonConfirm(textButton);

            if (!_isSubscription)
            {
                _isSubscription = true;
                AbilitySelectElementAction.ConfimSelect += ConfimSelect;
            }
        }
        
        private void ConfimSelect()
        {
            _isSubscription = false;
            AbilitySelectElementAction.ConfimSelect -= ConfimSelect;
            ref var uiActionSelectCard = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.AbilitySelectElementUIMono;
            uiActionSelectCard.CloseWindow();
            AbilityCardAction.ConfimSelectElement?.Invoke();
        }
    }
}