using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.EnemyPassport;
using CyberNet.Core.Player;
using CyberNet.Core.UI;

namespace CyberNet.Core.AbilityCard
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilitySelectPlayerSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;
        private int _selectPlayerID;
        
        public void PreInit()
        {
            AbilitySelectElementAction.SelectEnemyPlayer += SelectEnemyPlayer;
            AbilitySelectElementAction.CancelSelectPlayer += CancelSelectPlayer;
        }
        
        private void SelectEnemyPlayer(AbilityType abilityType)
        {
            ref var boardGameUI = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;
            ref var actionConfig = ref _dataWorld.OneData<CardsConfig>().AbilityCard;
            actionConfig.TryGetValue(abilityType.ToString(), out var actionVisualConfig);
            
            boardGameUI.TaskPlayerPopupUIMono.OpenWindowSetLocalizeTerm(actionVisualConfig.SelectPlayerFrameHeader, actionVisualConfig.SelectPlayerFrameDescr);
            boardGameUI.CoreHudUIMono.OnSelectPlayer();
            
            EnemyPassportAction.SelectPlayer += SelectPlayer;
        }
        
        private void SelectPlayer(int targetPlayerID)
        {
            EnemyPassportAction.SelectPlayer -= SelectPlayer;
            AbilityCardAction.SelectPlayer?.Invoke(targetPlayerID);
            OffSelectEnemyPlayer();
        }
        
        private void OffSelectEnemyPlayer()
        {
            ref var boardGameUI = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;
            boardGameUI.TaskPlayerPopupUIMono.CloseWindow();
            boardGameUI.CoreHudUIMono.OffSelectPlayer();

            var entitySelectAbilityTarget = _dataWorld.Select<CardComponent>()
                .With<AbilitySelectElementComponent>()
                .SelectFirstEntity();
            entitySelectAbilityTarget.RemoveComponent<AbilitySelectElementComponent>();
        }

        private void CancelSelectPlayer()
        {
            ref var boardGameUI = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;
            boardGameUI.TaskPlayerPopupUIMono.CloseWindow();
            boardGameUI.CoreHudUIMono.OffSelectPlayer();
            EnemyPassportAction.SelectPlayer -= SelectPlayer;
        }
    }
}