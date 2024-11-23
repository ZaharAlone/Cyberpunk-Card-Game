using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.Battle.TacticsMode;
using CyberNet.Core.Map;
using CyberNet.Core.Map.InteractiveElement;
using CyberNet.Core.MapMoveUnit;
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
            //Отступает один, т.к. только один проигравший может быть
            var playerRetreatEntities = _dataWorld.Select<SquadMustRetreatComponent>()
                .SelectFirstEntity();

            if (playerRetreatEntities.HasComponent<PlayerCurrentDeviceControlComponent>())
            {
                CurrentPlayerRetreat();
                return;
            }

            var playerComponent = playerRetreatEntities.GetComponent<PlayerInBattleComponent>();
            
            if (playerComponent.PlayerControlEntity != PlayerOrAI.Player)
                RetreatAI();
        }

        private void CurrentPlayerRetreat()
        {
             var playerEntity = _dataWorld.Select<SquadMustRetreatComponent>()
                .SelectFirstEntity();
            var playerComponent = playerEntity.GetComponent<PlayerInBattleComponent>();
            var currentBattleDistrictGUID = _dataWorld.OneData<BattleCurrentData>().DistrictBattleGUID;

            var districtComponent = _dataWorld.Select<DistrictComponent>()
                .Where<DistrictComponent>(district => district.GUID == currentBattleDistrictGUID)
                .SelectFirst<DistrictComponent>();
            
            var playerUnitsInDistrict = _dataWorld.Select<UnitMapComponent>()
                .Where<UnitMapComponent>(unit => unit.GUIDDistrict == districtComponent.GUID
                && unit.PowerSolidPlayerID == playerComponent.PlayerID)
                .GetEntities();

            foreach (var unitInDistrictEntity in playerUnitsInDistrict)
                unitInDistrictEntity.AddComponent(new SelectUnitMapComponent());

            playerEntity.AddComponent(new FollowClickDistrictComponent());
            playerEntity.AddComponent(new MoveUnitComponent ());
            
            CityAction.ShowWhereThePlayerCanRetreat?.Invoke(currentBattleDistrictGUID, playerComponent.PlayerID);
            CityAction.SelectDistrict += PlayerSelectTargetRetreatDistrict;
        }

        private void PlayerSelectTargetRetreatDistrict(string targetDistrictGUID)
        {
            CityAction.SelectDistrict -= PlayerSelectTargetRetreatDistrict;
            CityAction.DeactivateAllTower?.Invoke();

            var playerEntity = _dataWorld.Select<SquadMustRetreatComponent>()
                .SelectFirstEntity();

            playerEntity.RemoveComponent<FollowClickDistrictComponent>();
            ref var moveUnitComponent = ref playerEntity.GetComponent<MoveUnitComponent>();
            moveUnitComponent.TargetToMoveDistrictGUID = targetDistrictGUID;
            
            BattleAction.EndMovePlayerToNewDistrict += EndMoveUnit;
            MapMoveUnitsAction.StartMoveUnits?.Invoke();
        }

        private void RetreatAI()
        {
            var aiPlayerEntity = _dataWorld.Select<SquadMustRetreatComponent>()
                .SelectFirstEntity();
            var aiPlayerComponent = aiPlayerEntity.GetComponent<PlayerInBattleComponent>();
            var currentBattleDistrictGUID = _dataWorld.OneData<BattleCurrentData>().DistrictBattleGUID;

            var districtComponent = _dataWorld.Select<DistrictComponent>()
                .Where<DistrictComponent>(district => district.GUID == currentBattleDistrictGUID)
                .SelectFirst<DistrictComponent>();

            var minCountUnitsInNeighboringArea = 100;
            var targetToMoveDistrictGUID = "";
            
            foreach (var districtConnectMono in districtComponent.DistrictMono.ZoneConnect)
            {
                var connectDistrictComponent = _dataWorld.Select<DistrictComponent>()
                    .Where<DistrictComponent>(district => district.GUID == districtConnectMono.GUID)
                    .SelectFirst<DistrictComponent>();

                var countUnitsInDistrictControlSelectPlayer = _dataWorld.Select<UnitMapComponent>()
                    .Where<UnitMapComponent>(unit => unit.PowerSolidPlayerID == aiPlayerComponent.PlayerID
                    && unit.GUIDDistrict == connectDistrictComponent.GUID)
                    .Count();
                
                if (countUnitsInDistrictControlSelectPlayer == 0)
                    continue;
                
                if (countUnitsInDistrictControlSelectPlayer < minCountUnitsInNeighboringArea)
                {
                    minCountUnitsInNeighboringArea = countUnitsInDistrictControlSelectPlayer;
                    targetToMoveDistrictGUID = connectDistrictComponent.GUID;
                }
            }
            
            var aiUnitsInDistrictEntities = _dataWorld.Select<UnitMapComponent>()
                .Where<UnitMapComponent>(unit => unit.GUIDDistrict == currentBattleDistrictGUID
                && unit.PowerSolidPlayerID == aiPlayerComponent.PlayerID)
                .GetEntities();

            aiPlayerEntity.AddComponent(new MoveUnitComponent {
                PlayerID = aiPlayerComponent.PlayerID,
                TargetToMoveDistrictGUID = targetToMoveDistrictGUID,
            });

            foreach (var unitEntity in aiUnitsInDistrictEntities)
                unitEntity.AddComponent(new SelectUnitMapComponent());
            
            BattleAction.EndMovePlayerToNewDistrict += EndMoveUnit;
            MapMoveUnitsAction.StartMoveUnits?.Invoke();
        }

        private void EndMoveUnit(string _)
        {
            BattleAction.EndMovePlayerToNewDistrict -= EndMoveUnit;
            var playerEntity = _dataWorld.Select<SquadMustRetreatComponent>()
                .SelectFirstEntity();
            playerEntity.RemoveComponent<SquadMustRetreatComponent>();
        }

        public void Destroy()
        {
            BattleAction.EndMovePlayerToNewDistrict -= EndMoveUnit;
            BattleAction.EndAnimationsKillUnitsInMap -= CheckSquadRetreat;
        }
    }
}