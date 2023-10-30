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
    public class AbilitySelectElementSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AbilitySelectElementAction.OpenSelectAbilityCard += OpenWindow;
        }

        private void OpenWindow(int indexDescr)
        {
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();

            ref var playerComponent = ref playerEntity.GetComponent<PlayerComponent>();
            if (playerComponent.playerTypeEnum != PlayerTypeEnum.Player)
                return;
            
            var entity = _dataWorld.Select<CardComponent>().With<AbilitySelectElementComponent>().SelectFirstEntity();
            ref var actionSelectCardComponent = ref entity.GetComponent<AbilitySelectElementComponent>();
            ref var uiActionSelectCard = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.AbilitySelectElementUIMono;
            ref var abilityConfig = ref _dataWorld.OneData<CardsConfig>().AbilityCard;
            
            abilityConfig.TryGetValue(actionSelectCardComponent.AbilityCard.AbilityType.ToString(), out var actionVisualConfig);

            if (indexDescr == 0)
            {
                uiActionSelectCard.SetView(actionVisualConfig.SelectFrameHeader, actionVisualConfig.SelectFrameDescr);   
            }
            else
            {
                uiActionSelectCard.SetView(actionVisualConfig.SelectFrameHeader, actionVisualConfig.SelectFrameDescr_2);
            }
            
            //Зависит от абилки, дописать
            var isEnableCancelButton = true;
            uiActionSelectCard.OpenWindow(true);
            
            SubscriptionAction(actionSelectCardComponent.AbilityCard.AbilityType);
        }

        private void SubscriptionAction(AbilityType abilityType)
        {
            switch (abilityType)
            {
                case AbilityType.DestroyCard:
                    break;
                case AbilityType.CloneCard:
                    break;
                case AbilityType.DestroyTradeCard:
                    break;
                case AbilityType.SwitchNeutralSquad:
                    break;
                case AbilityType.SwitchEnemySquad:
                    break;
                case AbilityType.DestroyNeutralSquad:
                    break;
                case AbilityType.DestroySquad:
                    break;
                case AbilityType.SquadMove:
                    AbilitySelectElementAction.SelectTower += SelectTower;
                    break;
                case AbilityType.SetIce:
                    break;
                case AbilityType.DestroyIce:
                    break;
            }
        }
        
        private void SelectTower(string towerGUID)
        {
            ref var uiActionSelectCard = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.AbilitySelectElementUIMono;
            uiActionSelectCard.SetTextButtonConfirm(uiActionSelectCard.ConfimLocButton);
            
            AbilitySelectElementAction.ConfimSelect += ConfimSelectTower;
        }
        
        private void ConfimSelectTower()
        {
            AbilitySelectElementAction.SelectTower -= SelectTower;
            AbilitySelectElementAction.ConfimSelect -= ConfimSelectTower;
            
            var entity = _dataWorld.Select<CardComponent>().With<AbilitySelectElementComponent>().SelectFirstEntity();
            ref var actionSelectCardComponent = ref entity.GetComponent<AbilitySelectElementComponent>();

            if (actionSelectCardComponent.AbilityCard.AbilityType == AbilityType.SquadMove)
            {
                
                return;
            }
        }
        
        
    }
}