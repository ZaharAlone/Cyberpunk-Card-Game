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
            BattleAction.CloseTacticsScreen += CloseTacticsScreen;
        }
        
        private void OpenTacticsUI(int playerID)
        {
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player => player.PlayerID == playerID)
                .SelectFirstEntity();

            var firstTactics = _dataWorld.OneData<BattleTacticsData>().BattleTactics[0];
            playerEntity.AddComponent(new OpenBattleTacticsUIComponent {CurrentSelectTacticsUI = firstTactics.Key});
            
            SetShowMapUI(false);
            SetViewAvatarPlayers();
            SetStatsPlayersInBattle();
            SetTacticsBarView();
            
            BattleTacticsUIAction.CreateCardTactics?.Invoke();

            var uiTactics = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.BattleTacticsModeUIMono;
            uiTactics.ShowTacticsUI();
        }

        private void CloseTacticsScreen()
        {
            var openBattleTacticsUIEntity = _dataWorld.Select<OpenBattleTacticsUIComponent>().SelectFirstEntity();
            openBattleTacticsUIEntity.RemoveComponent<OpenBattleTacticsUIComponent>();
            
            SetShowMapUI(true);
            
            var uiTactics = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.BattleTacticsModeUIMono;
            uiTactics.HideTacticsUI();
            BattleTacticsUIAction.DestroyCardTactics?.Invoke();
        }
        
        private void SetShowMapUI(bool isShow)
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
            var playerInBattleEntities = _dataWorld.Select<PlayerInBattleComponent>().GetEntities();

            foreach (var playerInBattleEntity in playerInBattleEntities)
            {
                var playerInBattleComponent = playerInBattleEntity.GetComponent<PlayerInBattleComponent>();
                SetStatsPlayer(playerInBattleComponent);
            }
        }

        private void SetStatsPlayer(PlayerInBattleComponent playerInBattleComponent)
        {
            var uiTactics = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.BattleTacticsModeUIMono;

            if (playerInBattleComponent.IsAttacking)
            {
                var powerCount = playerInBattleComponent.PowerPoint;
                var killCount = playerInBattleComponent.KillPoint;
                var defenceCount = playerInBattleComponent.DefencePoint;
                
                var powerString = powerCount.ToString();
                var killString = killCount.ToString();
                var defenceString = defenceCount.ToString();

                uiTactics.PlayerStatsContainer_Attack.SetStats(powerString, killString, defenceString);
            }
            else
            {
                var powerString = TransformDefendingValueStats(playerInBattleComponent.PowerPoint);
                var killString = TransformDefendingValueStats(playerInBattleComponent.KillPoint);
                var defenceString = TransformDefendingValueStats(playerInBattleComponent.DefencePoint);
                
                uiTactics.PlayerStatsContainer_Defence.SetStats(powerString, killString, defenceString);
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
            BattleAction.CloseTacticsScreen -= CloseTacticsScreen;
        }
    }
}