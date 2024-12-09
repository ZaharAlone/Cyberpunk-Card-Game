using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System.Collections.Generic;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.AI;
using CyberNet.Core.Battle;
using CyberNet.Core.Map;
using CyberNet.Core.Player;
using CyberNet.Core.UI;
using CyberNet.Global;
using CyberNet.Global.Cursor;
using DG.Tweening;

namespace CyberNet.Core.MapMoveUnit
{
    [EcsSystem(typeof(CoreModule))]
    public class MapMoveUnitsAnimationsSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;
        
        public void PreInit()
        {
            MapMoveUnitsAction.StartMoveUnits += StartMoveUnit;
        }
        
        private void StartMoveUnit()
        {
            var moveUnitComponent = _dataWorld.Select<MoveUnitComponent>()
                .SelectFirst<MoveUnitComponent>();
            var targetToMoveDistrictGUID = moveUnitComponent.TargetToMoveDistrictGUID;
            var playerID = moveUnitComponent.PlayerID;
            
            var selectUnitQuery = _dataWorld.Select<SelectUnitMapComponent>();
            var selectUnitEntities = selectUnitQuery.GetEntities();
            
            var targetDistrictComponent = _dataWorld.Select<DistrictComponent>()
                .Where<DistrictComponent>(district => district.GUID == targetToMoveDistrictGUID)
                .SelectFirstEntity()
                .GetComponent<DistrictComponent>();
            
            var targetSlotZone = SelectTargetZoneInTower(targetToMoveDistrictGUID, playerID);
            Debug.Log($"target slot zone {targetSlotZone}");
            var parentUnitTower = targetDistrictComponent.SquadZonesMono[targetSlotZone].transform;
            var allTargetPositions = new List<Vector3>();
            
            foreach (var unitEntity in selectUnitEntities)
            {
                unitEntity.RemoveComponent<SelectUnitMapComponent>();
                ref var unitComponent = ref unitEntity.GetComponent<UnitMapComponent>();

                var nextTargetPosition = SelectNextPositionsForUnit(allTargetPositions, targetToMoveDistrictGUID, targetSlotZone);
                unitEntity.AddComponent(new MoveUnitToTargetComponent {
                    TargetPosition = nextTargetPosition,
                    TargetDistrictGUID = targetToMoveDistrictGUID,
                    TargetSlotID = targetSlotZone,
                });
                
                unitComponent.GUIDDistrict = targetToMoveDistrictGUID;
                unitComponent.IndexPoint = targetSlotZone;
                
                unitComponent.IconsUnitInMapMono.OffSelectUnitEffect();
                unitComponent.UnitIconsGO.transform.SetParent(parentUnitTower);
            }
            
            AnimationMoveToTarget();
        }

        private int SelectTargetZoneInTower(string selectTowerForAttackGuid, int playerID)
        {
            var isEnemyUnitInTargetTower = _dataWorld.Select<UnitMapComponent>()
                .Where<UnitMapComponent>(unit => unit.GUIDDistrict == selectTowerForAttackGuid
                    && unit.PowerSolidPlayerID != playerID)
                .Count() > 0;
            
            var isUnitInTargetTower = _dataWorld.Select<UnitMapComponent>()
                .Where<UnitMapComponent>(unit => unit.GUIDDistrict == selectTowerForAttackGuid)
                .Count() > 0;
            
            var targetSlotZone = 0;
            
            if (isEnemyUnitInTargetTower)
                targetSlotZone = GetEnemySlotInTargetZone(selectTowerForAttackGuid, playerID);
            else if (isUnitInTargetTower)
                targetSlotZone = GetFriendlySlotInTargetZone(selectTowerForAttackGuid, playerID);
            
            return targetSlotZone;
        }

        private int GetFriendlySlotInTargetZone(string districtGUID, int playerID)
        {
            Debug.Log("Get friendly slot in target zone");
            var districtEntity = _dataWorld.Select<DistrictComponent>()
                .Where<DistrictComponent>(tower => tower.GUID == districtGUID)
                .SelectFirstEntity();
             var districtComponent = districtEntity.GetComponent<DistrictComponent>();
            
            var targetSquadZone = 0;
            foreach (var squadZone in districtComponent.SquadZonesMono)
            {
                var isFriendlySlot = _dataWorld.Select<UnitMapComponent>()
                    .Where<UnitMapComponent>(unit => unit.GUIDDistrict == districtGUID
                        && unit.IndexPoint == squadZone.Index
                        && unit.PowerSolidPlayerID == playerID)
                    .Count() > 0;

                if (isFriendlySlot)
                    break;
                else
                    targetSquadZone = squadZone.Index + 1;
            }
            
            return targetSquadZone;
        }
        
        private int GetEnemySlotInTargetZone(string districtGUID, int playerID)
        {
            Debug.Log("Get enemy slot in target zone");
            var districtEntity = _dataWorld.Select<DistrictComponent>()
                .Where<DistrictComponent>(district => district.GUID == districtGUID)
                .SelectFirstEntity();
            ref var districtComponent = ref districtEntity.GetComponent<DistrictComponent>();
            
            var targetSquadZone = 0;
            foreach (var squadZone in districtComponent.SquadZonesMono)
            {
                var isCloseSlot = _dataWorld.Select<UnitMapComponent>()
                    .Where<UnitMapComponent>(unit => unit.GUIDDistrict == districtGUID
                        && unit.IndexPoint == squadZone.Index
                        && unit.PowerSolidPlayerID != playerID)
                    .Count() > 0;

                if (isCloseSlot)
                    targetSquadZone = squadZone.Index + 1;
                else
                    break;
            }
            
            return targetSquadZone;
        }

        private Vector3 SelectNextPositionsForUnit(List<Vector3> positionAllPoint, string selectTowerForAttackGuid, int targetSlotZone)
        {
            var targetTowerComponent = _dataWorld.Select<DistrictComponent>()
                .Where<DistrictComponent>(tower => tower.GUID == selectTowerForAttackGuid)
                .SelectFirstEntity()
                .GetComponent<DistrictComponent>();
            
            var newTargetPositions = CitySupportStatic.SelectPosition
            (
                targetTowerComponent.SquadZonesMono[targetSlotZone].Collider,
                targetTowerComponent.SquadZonesMono[targetSlotZone].transform.position,
                positionAllPoint
            );

            return newTargetPositions;
        }
        
        private void AnimationMoveToTarget()
        {
            Debug.Log("animation move to target");
            var moveUnitToTargetEntities = _dataWorld.Select<MoveUnitToTargetComponent>().GetEntities();

            foreach (var unitEntity in moveUnitToTargetEntities)
            {
                var unitComponent = unitEntity.GetComponent<UnitMapComponent>();
                var unitMoveComponent = unitEntity.GetComponent<MoveUnitToTargetComponent>();

                var timeMove = 1;
                var sequence = DOTween.Sequence();
                sequence.Append(unitComponent.UnitIconsGO.transform.DOMove(unitMoveComponent.TargetPosition, timeMove));
                
                _dataWorld.NewEntity().AddComponent(new TimeComponent {
                    Time = timeMove,
                    Action = () => {
                        unitEntity.RemoveComponent<MoveUnitToTargetComponent>();
                        CheckFinishMoveUnit();
                    }
                });
            }
        }
        
        private void CheckFinishMoveUnit()
        {
            var moveUnitEntity = _dataWorld.Select<MoveUnitComponent>()
                .SelectFirstEntity();
            var moveUnitComponent = moveUnitEntity.GetComponent<MoveUnitComponent>();
            
            var allUnitFinishMove = _dataWorld.Select<MoveUnitToTargetComponent>().Count() == 0;
            
            if (allUnitFinishMove)
            {
                var isEnemyTargetZone = CheckIsEnemyInTargetMoveZone();
                if (isEnemyTargetZone)
                    BattleAction.EndMovePlayerToNewDistrict?.Invoke(moveUnitComponent.TargetToMoveDistrictGUID);
                else
                    EndMoveWithoutBattle();
                
                Debug.Log($"End move unit, enemy in target zone {isEnemyTargetZone}");
                moveUnitEntity.RemoveComponent<MoveUnitComponent>();
                if (moveUnitEntity.HasComponent<MoveUnitSelectTowerComponent>())
                    moveUnitEntity.RemoveComponent<MoveUnitSelectTowerComponent>();
            }
        }

        private bool CheckIsEnemyInTargetMoveZone()
        {
            var moveUnitComponent = _dataWorld.Select<MoveUnitComponent>()
                .SelectFirst<MoveUnitComponent>();

            var countEnemyUnit = _dataWorld.Select<UnitMapComponent>()
                .Where<UnitMapComponent>(unit => unit.GUIDDistrict == moveUnitComponent.TargetToMoveDistrictGUID
                    && unit.PowerSolidPlayerID != moveUnitComponent.PlayerID)
                .Count();

            var isEnemy = countEnemyUnit > 0;
            return isEnemy;
        }

        private void EndMoveWithoutBattle()
        {
            _dataWorld.OneData<RoundData>().PauseInteractive = false;
            BattleAction.EndMoveWithoutBattle?.Invoke();
            
            CustomCursorAction.OnBaseCursor?.Invoke();
            VFXCardInteractiveAction.UpdateVFXCard?.Invoke();
            CityAction.UpdateCanInteractiveMap?.Invoke();
            CityAction.UpdatePresencePlayerInCity?.Invoke();

            var currentPlayerType = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirst<PlayerComponent>()
                .playerOrAI;
            
            if (currentPlayerType != PlayerOrAI.Player)
                BotAIAction.ContinuePlayingCards?.Invoke();
        }

        public void Destroy()
        {
            MapMoveUnitsAction.StartMoveUnits -= StartMoveUnit;
        }
    }
}