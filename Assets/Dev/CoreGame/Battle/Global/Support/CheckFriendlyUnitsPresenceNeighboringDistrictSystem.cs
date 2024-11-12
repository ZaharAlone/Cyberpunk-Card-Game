using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.Battle.TacticsMode;
using CyberNet.Core.Map;

namespace CyberNet.Core.Battle
{
    [EcsSystem(typeof(CoreModule))]
    public class CheckFriendlyUnitsPresenceNeighboringDistrictSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            BattleAction.CheckBattleFriendlyUnitsPresenceNeighboringDistrict += CheckNeighboringDistrictWithFriendlyUnits;
        }
        
        private bool CheckNeighboringDistrictWithFriendlyUnits(int targetPlayerID)
        {
            var currentDistrictBattle = _dataWorld.OneData<BattleCurrentData>().DistrictBattleGUID;
            var districtComponent = _dataWorld.Select<DistrictComponent>()
                .Where<DistrictComponent>(district => district.GUID == currentDistrictBattle)
                .SelectFirst<DistrictComponent>();

            var isFriendlyUnitsDistrict = false;

            foreach (var districtConnectMono in districtComponent.DistrictMono.ZoneConnect)
            {
                var connectDistrictComponent = _dataWorld.Select<DistrictComponent>()
                    .Where<DistrictComponent>(district => district.GUID == districtConnectMono.GUID)
                    .SelectFirst<DistrictComponent>();

                if (connectDistrictComponent.DistrictBelongPlayerID == targetPlayerID)
                {
                    isFriendlyUnitsDistrict = true;
                    break;
                }
            }
            
            return isFriendlyUnitsDistrict;
        }

        public void Destroy()
        {
            BattleAction.CheckBattleFriendlyUnitsPresenceNeighboringDistrict -= CheckNeighboringDistrictWithFriendlyUnits;
        }
    }
}