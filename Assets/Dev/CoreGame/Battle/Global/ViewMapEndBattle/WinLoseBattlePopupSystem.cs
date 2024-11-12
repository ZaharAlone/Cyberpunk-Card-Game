using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.Battle;
using CyberNet.Core.UI;
using CyberNet.Global;

namespace CyberNet.Core.Battle
{
    [EcsSystem(typeof(CoreModule))]
    public class WinLoseBattlePopupSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            BattleAction.EndAnimationsKillUnitsInMap += ShowPopupWinLose;
        }

        private void ShowPopupWinLose()
        {
            var currentDevicePlayerEntity = _dataWorld.Select<PlayerCurrentDeviceControlComponent>().SelectFirstEntity();

            if (currentDevicePlayerEntity.HasComponent<PlayerWinBattleComponent>() || currentDevicePlayerEntity.HasComponent<PlayerLoseBattleComponent>())
            {
                ShowPopupWinLosePlayer();
            }
            else
            {
                BattleAction.ShowOtherPlayersBattleWinLoseInfo?.Invoke();
            }
        }

        private void ShowPopupWinLosePlayer()
        {
            var currentDevicePlayerEntity = _dataWorld.Select<PlayerCurrentDeviceControlComponent>().SelectFirstEntity();

            var winLoseUIPopup = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.WinLoseBattleUIMono;
            if (currentDevicePlayerEntity.HasComponent<PlayerWinBattleComponent>())
                winLoseUIPopup.ShowPopupWin();
            else
                winLoseUIPopup.ShowPopupLose();
        }

        public void Destroy()
        {
            BattleAction.EndAnimationsKillUnitsInMap -= ShowPopupWinLose;
        }
    }
}