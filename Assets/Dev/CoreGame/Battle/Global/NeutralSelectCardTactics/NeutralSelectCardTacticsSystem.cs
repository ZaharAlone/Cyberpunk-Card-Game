using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.Battle;

namespace CyberNet.Core.Battle.TacticsMode
{
    [EcsSystem(typeof(CoreModule))]
    public class NeutralSelectCardTacticsSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            BattleAction.SelectTacticsCardNeutralUnit += SelectTacticsCardNeutralUnit;
        }
        
        private void SelectTacticsCardNeutralUnit()
        {
            
        }

        public void Destroy()
        {
            BattleAction.SelectTacticsCardNeutralUnit -= SelectTacticsCardNeutralUnit;
        }
    }
}