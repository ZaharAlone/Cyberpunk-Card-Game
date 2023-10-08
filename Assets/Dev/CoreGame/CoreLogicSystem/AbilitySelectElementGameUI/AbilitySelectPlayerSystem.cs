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
            
            boardGameUI.AbilitySelectElementUIMono.SetView(actionVisualConfig.SelectPlayerFrameHeader, actionVisualConfig.SelectPlayerFrameDescr);
            boardGameUI.AbilitySelectElementUIMono.OpenWindow(true);
            boardGameUI.CoreHudUIMono.OnSelectPlayer();
            
            EnemyPassportAction.SelectPlayer += SelectPlayer;
        }
        
        private void SelectPlayer(int targetPlayerID)
        {
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player => player.PlayerID == targetPlayerID)
                .SelectFirstEntity();

            ref var playerViewComponent = ref playerEntity.GetComponent<PlayerViewComponent>();
            ref var boardGameUI = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;
            boardGameUI.AbilitySelectElementUIMono.SetTextButtonConfirm(playerViewComponent.Name);
            _selectPlayerID = targetPlayerID;
            
            AbilitySelectElementAction.ConfimSelect += ConfimSelectPlayer;
        }
        
        private void ConfimSelectPlayer()
        {
            EnemyPassportAction.SelectPlayer -= SelectPlayer;
            AbilitySelectElementAction.ConfimSelect -= ConfimSelectPlayer;
            AbilityCardAction.SelectPlayer?.Invoke(_selectPlayerID);
            OffSelectEnemyPlayer();
        }

        private void OffSelectEnemyPlayer()
        {
            ref var boardGameUI = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;
            boardGameUI.AbilitySelectElementUIMono.CloseWindow();
            boardGameUI.CoreHudUIMono.OffSelectPlayer();

            var entitySelectAbilityTarget = _dataWorld.Select<CardComponent>()
                .With<AbilitySelectElementComponent>()
                .SelectFirstEntity();
            entitySelectAbilityTarget.RemoveComponent<AbilitySelectElementComponent>();
        }

        private void CancelSelectPlayer()
        {
            ref var boardGameUI = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;
            boardGameUI.AbilitySelectElementUIMono.CloseWindow();
            boardGameUI.CoreHudUIMono.OffSelectPlayer();
            EnemyPassportAction.SelectPlayer -= SelectPlayer;
            AbilitySelectElementAction.ConfimSelect -= ConfimSelectPlayer;
        }
    }
}