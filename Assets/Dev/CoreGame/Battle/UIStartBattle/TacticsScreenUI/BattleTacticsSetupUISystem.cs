using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using CyberNet.Core.Player;
using CyberNet.Core.UI;
using CyberNet.Global;

namespace CyberNet.Core.Battle.TacticsMode
{
    [EcsSystem(typeof(CoreModule))]
    public class BattleTacticsSetupUISystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        private const string unknown_quantity_add_value = "+?";
        private const string unknown_quantity_value = "?";
        
        public void PreInit()
        {
            BattleAction.OpenTacticsScreen += OpenTacticsUI;
        }
        private void OpenTacticsUI(int playerID)
        {
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player => player.PlayerID == playerID)
                .SelectFirstEntity();

            playerEntity.AddComponent(new OpenBattleTacticsUIComponent());
            
            ShowHideMapUI(false);
            SetViewAvatarPlayers();
            SetStatsPlayersInBattle();
            SetTacticsBarView();
            
            BattleTacticsUIAction.CreateCardTactics?.Invoke();

            var uiTactics = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.BattleTacticsModeUIMono;
            uiTactics.ShowTacticsUI();
        }

        private void CloseTacticsUI()
        {
            _dataWorld.Select<OpenBattleTacticsUIComponent>().SelectFirstEntity().Destroy();
            ShowHideMapUI(true);

            var openTacticsScreenEntity = _dataWorld.Select<OpenBattleTacticsUIComponent>().SelectFirstEntity();
            openTacticsScreenEntity.RemoveComponent<OpenBattleTacticsUIComponent>();
        }
        
        private void ShowHideMapUI(bool isShow)
        {
            var boardGameUI = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;

            if (isShow)
            {
                boardGameUI.CoreHudUIMono.ShowCurrentPlayer();
                boardGameUI.CoreHudUIMono.ShowEnemyPassport();
                boardGameUI.CoreHudUIMono.ShowButtons();
                boardGameUI.TraderowMono.EnableTradeRow();
                ActionPlayerButtonEvent.UpdateActionButton?.Invoke();
            }
            else
            {
                boardGameUI.CoreHudUIMono.HideCurrentPlayer();
                boardGameUI.CoreHudUIMono.HideEnemyPassport();
                boardGameUI.CoreHudUIMono.HideButtons();
                boardGameUI.TraderowMono.DisableTradeRow();
                ActionPlayerButtonEvent.SetViewBattle?.Invoke();
            }
            
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;

            var cardInPlayerHandEntities = _dataWorld.Select<CardComponent>()
                .With<CardHandComponent>()
                .Where<CardComponent>(card => card.PlayerID == currentPlayerID)
                .GetEntities();

            foreach (var entityDiscardCard in cardInPlayerHandEntities)
            {
                var cardMono = entityDiscardCard.GetComponent<CardComponent>().CardMono;
                if (isShow)
                    cardMono.ShowCard();
                else
                    cardMono.HideCard();
            }
        }

        private void SetViewAvatarPlayers()
        {
            var uiTactics = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.BattleTacticsModeUIMono;
            var playerInBattleEntities = _dataWorld.Select<PlayerInBattleComponent>().GetEntities();

            foreach (var playerInBattleEntity in playerInBattleEntities)
            {
                var playerInBattleComponent = playerInBattleEntity.GetComponent<PlayerInBattleComponent>();
                var playerAvatar = GetPlayerAvatar(playerInBattleComponent.PlayerID);
                
                if (playerInBattleComponent.IsAttacking)
                    uiTactics.PlayerStatsContainer_Attack.SetAvatarPlayer(playerAvatar);
                else
                    uiTactics.PlayerStatsContainer_Defence.SetAvatarPlayer(playerAvatar);
            }
        }

        private Sprite GetPlayerAvatar(int playerID)
        {
            if (playerID == -1)
            {
                var neutralAvatar = _dataWorld.OneData<LeadersViewData>().NeutralLeaderAvatar;
                return neutralAvatar;
            }
            else
            {
                var avatarPlayer = _dataWorld.Select<PlayerComponent>()
                    .Where<PlayerComponent>(player => player.PlayerID == playerID)
                    .SelectFirst<PlayerViewComponent>().AvatarForBattle;
                return avatarPlayer;
            }
        }

        private void SetStatsPlayersInBattle()
        {
            var uiTactics = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.BattleTacticsModeUIMono;
            var playerInBattleEntities = _dataWorld.Select<PlayerInBattleComponent>().GetEntities();

            foreach (var playerInBattleEntity in playerInBattleEntities)
            {
                var playerInBattleComponent = playerInBattleEntity.GetComponent<PlayerInBattleComponent>();
                SetStatsPlayer(playerInBattleComponent, uiTactics.PlayerStatsContainer_Attack);
                SetStatsPlayer(playerInBattleComponent, uiTactics.PlayerStatsContainer_Defence);
            }
        }

        private void SetStatsPlayer(PlayerInBattleComponent playerInBattleComponent, BattlePlayerStatsContainerUIMono uiContainer)
        {
            if (!playerInBattleComponent.IsAttacking)
            {
                var powerString = TransformDefendingValueStats(playerInBattleComponent.PowerPoint.BaseValue);
                var killString = TransformDefendingValueStats(playerInBattleComponent.KillPoint.BaseValue);
                var defenceString = TransformDefendingValueStats(playerInBattleComponent.DefencePoint.BaseValue);
                
                uiContainer.SetStats(powerString, killString, defenceString);
            }
            else
            {
                //TODO calculate card value
                var powerCount = playerInBattleComponent.PowerPoint.BaseValue + playerInBattleComponent.PowerPoint.AbilityValue;
                var killCount = playerInBattleComponent.KillPoint.BaseValue + playerInBattleComponent.KillPoint.AbilityValue;
                var defenceCount = playerInBattleComponent.DefencePoint.BaseValue + playerInBattleComponent.DefencePoint.AbilityValue;
                
                var powerString = powerCount.ToString();
                var killString = killCount.ToString();
                var defenceString = defenceCount.ToString();
                
                uiContainer.SetStats(powerString, killString, defenceString);
            }
        }

        private string TransformDefendingValueStats(int value)
        {
            var valueString = "";
            if (value == 0)
                valueString = unknown_quantity_value;
            else
                valueString = value + unknown_quantity_add_value;
            return valueString;
        }

        private void SetTacticsBarView()
        {
            var uiTactics = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.BattleTacticsModeUIMono;
            var battleTactics = _dataWorld.OneData<BattleTacticsData>().BattleTactics;
            var currencyIconsConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig.CurrencyImage;

            var indexCurrentSlotTactics = 0;
            foreach (var currentTactics in battleTactics)
            {
                var leftIcons = currencyIconsConfig[currentTactics.LeftCharacteristics.ToString()];
                var rightIcons = currencyIconsConfig[currentTactics.RightCharacteristics.ToString()];

                uiTactics.BattleTacticsSlotList[indexCurrentSlotTactics].SetView(currentTactics.Key, leftIcons, rightIcons);
                
                indexCurrentSlotTactics++;
            }

            ref var openTacticsScreenComponent = ref _dataWorld.Select<OpenBattleTacticsUIComponent>().SelectFirst<OpenBattleTacticsUIComponent>();
            openTacticsScreenComponent.CurrentSelectTacticsUI = battleTactics[0].Key;
        }
        
        public void Destroy()
        {
            BattleAction.OpenTacticsScreen -= OpenTacticsUI;
        }
    }
}