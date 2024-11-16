using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.Battle.TacticsMode;
using CyberNet.Core.Map;
using CyberNet.Global;

namespace CyberNet.Core.Battle
{
    [EcsSystem(typeof(CoreModule))]
    public class SquadRetreatSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            BattleAction.EndAnimationsKillUnitsInMap += CheckSquadRetreat;
        }

        private void CheckSquadRetreat()
        {
            var isSquadRetreat = _dataWorld.Select<SquadMustRetreatComponent>().Count() > 0;
            if (isSquadRetreat)
                SquadRetreat();
        }
        
        private void SquadRetreat()
        {
            var playerRetreatEntity = _dataWorld.Select<SquadMustRetreatComponent>()
                .SelectFirstEntity();

            if (playerRetreatEntity.HasComponent<PlayerCurrentDeviceControlComponent>())
            {
                CurrentPlayerRetreat();
                return;
            }

            var playerComponent = playerRetreatEntity.GetComponent<PlayerInBattleComponent>();
            
            if (playerComponent.PlayerControlEntity != PlayerOrAI.Player)
                RetreatAI();
        }

        private void CurrentPlayerRetreat()
        {
            
        }

        private void RetreatAI()
        {
            var playerComponent = _dataWorld.Select<SquadMustRetreatComponent>()
                .SelectFirst<PlayerInBattleComponent>();
            var currentBattleDistrictGUID = _dataWorld.OneData<BattleCurrentData>().DistrictBattleGUID;

            var districtComponent = _dataWorld.Select<DistrictComponent>()
                .Where<DistrictComponent>(district => district.GUID == currentBattleDistrictGUID)
                .SelectFirst<DistrictComponent>();

            var minCountUnitsInNeighboringArea = 100;
            var targetGUIDDistrict = "";
            
            foreach (var districtConnectMono in districtComponent.DistrictMono.ZoneConnect)
            {
                var connectDistrictComponent = _dataWorld.Select<DistrictComponent>()
                    .Where<DistrictComponent>(district => district.GUID == districtConnectMono.GUID)
                    .SelectFirst<DistrictComponent>();

                var countUnitsInDistrictControlSelectPlayer = _dataWorld.Select<UnitMapComponent>()
                    .Where<UnitMapComponent>(unit => unit.PowerSolidPlayerID == playerComponent.PlayerID)
                    .Count();
                
                if (countUnitsInDistrictControlSelectPlayer == 0)
                    continue;

                if (countUnitsInDistrictControlSelectPlayer < minCountUnitsInNeighboringArea)
                {
                    minCountUnitsInNeighboringArea = countUnitsInDistrictControlSelectPlayer;
                    targetGUIDDistrict = connectDistrictComponent.GUID;
                }
            }
            
            var playerUnitsInDistrict = _dataWorld.Select<UnitMapComponent>().GetEntities();
            
        }

        public void Destroy()
        {
            BattleAction.EndAnimationsKillUnitsInMap -= CheckSquadRetreat;
        }
    }
}