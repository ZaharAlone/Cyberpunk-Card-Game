using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using System.Collections.Generic;
using CyberNet.Core.UI;

namespace CyberNet.Core.Battle.TacticsMode.InteractiveCard
{
    [EcsSystem(typeof(CoreModule))]
    public class SelectBattleTacticsInteractiveSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            BattleTacticsUIAction.SelectBattleTactics += SelectBattleTactics;
            BattleTacticsUIAction.NextLeftBattleTactics += NextLeftBattleTactics;
            BattleTacticsUIAction.NextRightBattleTactics += NextRightBattleTactics;
        }

        private void NextLeftBattleTactics()
        {
            var openTacticsScreenComponent = _dataWorld.Select<OpenBattleTacticsUIComponent>()
                .SelectFirst<OpenBattleTacticsUIComponent>();
            var battleTactics = _dataWorld.OneData<BattleTacticsData>().BattleTactics;

            var listTactics = new List<string>();
            foreach (var currentTactics in battleTactics)
                listTactics.Add(currentTactics.Key);
            
            var targetIndex = listTactics.IndexOf(openTacticsScreenComponent.CurrentSelectTacticsUI);

            var nextIndex = targetIndex - 1;
            if (nextIndex < 0)
                nextIndex = listTactics.Count - 1;
            
            var nextTacticsKey = listTactics[nextIndex];
            SelectBattleTactics(nextTacticsKey);
        }
        
        private void NextRightBattleTactics()
        {
            var openTacticsScreenComponent = _dataWorld.Select<OpenBattleTacticsUIComponent>()
                .SelectFirst<OpenBattleTacticsUIComponent>();
            var battleTactics = _dataWorld.OneData<BattleTacticsData>().BattleTactics;

            var listTactics = new List<string>();
            foreach (var currentTactics in battleTactics)
                listTactics.Add(currentTactics.Key);
            
            var targetIndex = listTactics.IndexOf(openTacticsScreenComponent.CurrentSelectTacticsUI);

            var nextIndex = targetIndex + 1;
            if (nextIndex > listTactics.Count - 1)
                nextIndex = 0;
            
            var nextTacticsKey = listTactics[nextIndex];
            SelectBattleTactics(nextTacticsKey);
        }

        private void SelectBattleTactics(string keyTactics)
        {
            var uiTactics = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.BattleTacticsModeUIMono;
            var battleTactics = _dataWorld.OneData<BattleTacticsData>().BattleTactics;
            var currencyIconsConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig.CurrencyImage;

            var sortingTactics = SortingTactics(keyTactics);
            if (sortingTactics == null)
                return;
            
            var indexCurrentSlotTactics = 0;
            foreach (var currentTacticsKey in sortingTactics)
            {
                foreach (var tacticConfig in battleTactics)
                {
                    if (tacticConfig.Key == currentTacticsKey)
                    {
                        var leftIcons = currencyIconsConfig[tacticConfig.LeftCharacteristics.ToString()];
                        var rightIcons = currencyIconsConfig[tacticConfig.RightCharacteristics.ToString()];

                        uiTactics.BattleTacticsSlotList[indexCurrentSlotTactics].SwitchView(currentTacticsKey, leftIcons, rightIcons);
                
                        indexCurrentSlotTactics++;
                        break;
                    }
                }
            }

            ref var openTacticsScreenComponent = ref _dataWorld.Select<OpenBattleTacticsUIComponent>()
                .SelectFirst<OpenBattleTacticsUIComponent>();

            openTacticsScreenComponent.CurrentSelectTacticsUI = sortingTactics[0];
            
            BattleTacticsUIAction.UpdateCardAndTactics?.Invoke();
        }

        private List<string> SortingTactics(string targetKeyTactics)
        {
            var battleTactics = _dataWorld.OneData<BattleTacticsData>().BattleTactics;
            var listTactics = new List<string>();
            foreach (var currentTactics in battleTactics)
                listTactics.Add(currentTactics.Key);
            
            var targetIndex = listTactics.IndexOf(targetKeyTactics);

            if (targetIndex == -1)
            {
                Debug.LogError($"Not find target tactics in list, key error {targetKeyTactics}");
                return null;
            }
            
            var sortingListTactics = new List<string>();
            sortingListTactics.AddRange(listTactics.GetRange(targetIndex, listTactics.Count - targetIndex));
            sortingListTactics.AddRange(listTactics.GetRange(0, targetIndex));
            return sortingListTactics;
        }
        
        public void Destroy()
        {
            BattleTacticsUIAction.SelectBattleTactics -= SelectBattleTactics;
            BattleTacticsUIAction.NextLeftBattleTactics -= NextLeftBattleTactics;
            BattleTacticsUIAction.NextRightBattleTactics -= NextRightBattleTactics;
        }
    }
}