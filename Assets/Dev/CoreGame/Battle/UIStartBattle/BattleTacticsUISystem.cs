using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using System.Collections.Generic;
using CyberNet.Core.Arena;
using CyberNet.Core.Map;
using CyberNet.Core.UI;

namespace CyberNet.Core.Battle.TacticsMode
{
    [EcsSystem(typeof(CoreModule))]
    public class BattleTacticsUISystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            BattleAction.OpenTacticsScreen += EnableTacticsUI;
        }
        private void EnableTacticsUI()
        {
            var uiTactics = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.BattleTacticsModeUIMono;
            
            
        }

        public void Destroy()
        {
            BattleAction.OpenTacticsScreen -= EnableTacticsUI;
        }
    }
}