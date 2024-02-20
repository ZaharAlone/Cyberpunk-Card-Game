using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.UI;
using CyberNet.Meta.EndGame;

namespace CyberNet.Core.WinLose
{
    [EcsSystem(typeof(CoreModule))]
    public class WinLoseSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            WinLoseAction.CloseScreen += CloseWindow;
            WinLoseAction.CheckWin += CheckWin;
        }
        private void CheckWin()
        {
            //TODO: вернуть
            /*
            ref var player1Stats = ref _dataWorld.OneData<PlayerStatsComponent>();
            ref var player2Stats = ref _dataWorld.OneData<Player2StatsData>();

            if (player1Stats.HP <= 0)
            {
                OpenWinScreen(PlayerEnum.Player1);
            }
            else if (player2Stats.HP <= 0)
            {
                OpenWinScreen(PlayerEnum.Player2);
            }*/
        }
/*
        public void OpenWinScreen(PlayerEnum playerWin)
        {
            ref var uiWinLoseScreen = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.WinLoseUIMono;
            ref var player1View = ref _dataWorld.OneData<PlayerViewComponent>();
            ref var player2View = ref _dataWorld.OneData<Player2ViewData>();
            ref var leadersViewData = ref _dataWorld.OneData<LeadersViewData>().LeadersView;
            leadersViewData.TryGetValue(player1View.AvatarKey, out var avatarPlayer1);
            leadersViewData.TryGetValue(player2View.AvatarKey, out var avatarPlayer2);

            if (playerWin == PlayerEnum.Player1)
            {
                uiWinLoseScreen.OpenWindow(avatarPlayer1, avatarPlayer2);
            }
            else
            {
                uiWinLoseScreen.OpenWindow(avatarPlayer2, avatarPlayer1);
            }
        }
*/
        private void CloseWindow()
        {
            EndGameAction.EndGame?.Invoke();
        }

        public void Destroy()
        {
            WinLoseAction.CloseScreen -= CloseWindow;
            WinLoseAction.CheckWin -= CheckWin;
        }
    }
}