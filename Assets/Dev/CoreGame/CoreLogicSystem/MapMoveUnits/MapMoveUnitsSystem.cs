using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System.Collections.Generic;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.Battle;
using CyberNet.Core.Player;
using CyberNet.Core.UI;
using CyberNet.Global;
using CyberNet.Global.Cursor;
using DG.Tweening;

namespace CyberNet.Core.Map
{
    [EcsSystem(typeof(CoreModule))]
    public class MapMoveUnitsSystem : IPreInitSystem, IInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;
        
        public void PreInit()
        {
            MapMoveUnitsAction.StartMoveUnits += StartMoveUnit;
        }
        
        public void Init()
        {
            _dataWorld.CreateOneData<MoveUnitZoneData>();
        }
        
        private void StartMoveUnit()
        {
            var selectTowerForAttackGuid = _dataWorld.Select<AbilityCardMoveUnitComponent>()
                .SelectFirstEntity()
                .GetComponent<AbilityCardMoveUnitComponent>()
                .SelectTowerGUID;

            var selectUnitEntities = _dataWorld.Select<SelectUnitMapComponent>().GetEntities();
            var targetTowerComponent = _dataWorld.Select<DistrictComponent>()
                .Where<DistrictComponent>(tower => tower.GUID == selectTowerForAttackGuid)
                .SelectFirstEntity()
                .GetComponent<DistrictComponent>();
            
            var targetSlotZone = SelectTargetZoneInTower(selectTowerForAttackGuid);

            var parentUnitTower = targetTowerComponent.SquadZonesMono[targetSlotZone].transform;
            
            var allTargetPositions = new List<Vector3>();
            var nextTargetPosition = SelectNextPositionsForUnit(allTargetPositions, selectTowerForAttackGuid, targetSlotZone);
            
            foreach (var unitEntity in selectUnitEntities)
            {
                unitEntity.RemoveComponent<SelectUnitMapComponent>();
                ref var unitComponent = ref unitEntity.GetComponent<UnitMapComponent>();
                
                unitEntity.AddComponent(new MoveUnitToTargetComponent {
                    TargetPosition = nextTargetPosition,
                    TargetTowerGUID = selectTowerForAttackGuid,
                    TargetSlotID = targetSlotZone,
                });

                _dataWorld.OneData<MoveUnitZoneData>().LastTargetTowerGUID = selectTowerForAttackGuid;

                unitComponent.GUIDTower = selectTowerForAttackGuid;
                unitComponent.IndexPoint = targetSlotZone;
                
                unitComponent.IconsUnitInMapMono.OffSelectUnitEffect();
                unitComponent.UnitIconsGO.transform.SetParent(parentUnitTower);
            }
            
            AnimationMoveToTarget();
        }

        private int SelectTargetZoneInTower(string selectTowerForAttackGuid)
        {
            var isEnemyUnitInTargetTower = _dataWorld.Select<UnitMapComponent>()
                .Where<UnitMapComponent>(unit => unit.GUIDTower == selectTowerForAttackGuid).Count() > 0;
            var selectUnitEntities = _dataWorld.Select<SelectUnitMapComponent>().GetEntities();
            
            var targetSlotZone = 0;
            
            if (isEnemyUnitInTargetTower)
            {
                var entitiesUnitEntityIsDefensePosition = _dataWorld.Select<UnitMapComponent>()
                    .Where<UnitMapComponent>(unit => unit.GUIDTower == selectTowerForAttackGuid)
                    .GetEntities();
                
                foreach (var entity in entitiesUnitEntityIsDefensePosition)
                {
                    entity.AddComponent(new UnitInBattleArenaComponent {
                        Attacking = false
                    });
                }
                
                foreach (var entity in selectUnitEntities)
                {
                    entity.AddComponent(new UnitInBattleArenaComponent {
                        Attacking = true
                    });
                }

                targetSlotZone = GetEnemySlotInTargetZone(selectTowerForAttackGuid);
            }
            else
            {
                targetSlotZone = GetFriendlySlotInTargetZone(selectTowerForAttackGuid);
            }
            return targetSlotZone;
        }

        private int GetFriendlySlotInTargetZone(string towerGUID)
        {
            var currentPlayerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();
            var currentPlayerComponent = currentPlayerEntity.GetComponent<PlayerComponent>();
            
            var towerEntity = _dataWorld.Select<DistrictComponent>()
                .Where<DistrictComponent>(tower => tower.GUID == towerGUID)
                .SelectFirstEntity();
            ref var towerComponent = ref towerEntity.GetComponent<DistrictComponent>();
            
            var targetSquadZone = 0;
            foreach (var squadZone in towerComponent.SquadZonesMono)
            {
                var isFriendlySlot = _dataWorld.Select<UnitMapComponent>()
                    .Where<UnitMapComponent>(unit => unit.GUIDTower == towerGUID
                        && unit.IndexPoint == squadZone.Index
                        && unit.PowerSolidPlayerID == currentPlayerComponent.PlayerID)
                    .Count() > 0;

                if (isFriendlySlot)
                    break;
                else
                {
                    targetSquadZone = squadZone.Index + 1;
                }
            }
            
            return targetSquadZone;
        }
        
        private int GetEnemySlotInTargetZone(string towerGUID)
        {
            var currentPlayerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();
            var currentPlayerComponent = currentPlayerEntity.GetComponent<PlayerComponent>();
            
            var towerEntity = _dataWorld.Select<DistrictComponent>()
                .Where<DistrictComponent>(tower => tower.GUID == towerGUID)
                .SelectFirstEntity();
            ref var towerComponent = ref towerEntity.GetComponent<DistrictComponent>();
            
            var targetSquadZone = 0;
            foreach (var squadZone in towerComponent.SquadZonesMono)
            {
                var isCloseSlot = _dataWorld.Select<UnitMapComponent>()
                    .Where<UnitMapComponent>(unit => unit.GUIDTower == towerGUID
                        && unit.IndexPoint == squadZone.Index
                        && unit.PowerSolidPlayerID != currentPlayerComponent.PlayerID)
                    .Count() > 0;

                if (isCloseSlot)
                    targetSquadZone = squadZone.Index + 1;
                else
                {
                    break;
                }
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
            var isMoveUnit = _dataWorld.Select<MoveUnitToTargetComponent>().Count() > 0;
            
            if (!isMoveUnit)
            {
                if (CheckIsEnemyInTargetMoveZone())
                    BattleAction.EndMovePlayerToNewZone?.Invoke();
                else
                    EndMoveWithoutBattle();
            }
        }

        private bool CheckIsEnemyInTargetMoveZone()
        {
            var isEnemy = false;

            var lastTargetTowerGUID = _dataWorld.OneData<MoveUnitZoneData>().LastTargetTowerGUID;
            if (lastTargetTowerGUID != "")
            {
                var currentPlayerID = _dataWorld.Select<PlayerComponent>()
                    .With<CurrentPlayerComponent>()
                    .SelectFirst<PlayerComponent>().PlayerID;
                
                var isEnemyUnit = _dataWorld.Select<UnitMapComponent>()
                    .Where<UnitMapComponent>(unit => unit.GUIDTower == lastTargetTowerGUID
                    && unit.PowerSolidPlayerID != currentPlayerID)
                    .Count();

                isEnemy = isEnemyUnit > 0;
            }

            return isEnemy;
        }

        private void EndMoveWithoutBattle()
        {
            _dataWorld.OneData<RoundData>().PauseInteractive = false;
            
            CustomCursorAction.OnBaseCursor?.Invoke();
            VFXCardInteractiveAction.UpdateVFXCard?.Invoke();
            CityAction.UpdateCanInteractiveMap?.Invoke();
            CityAction.UpdatePresencePlayerInCity?.Invoke();
        }

        public void Destroy()
        {
            MapMoveUnitsAction.StartMoveUnits -= StartMoveUnit;
            
            _dataWorld.RemoveOneData<MoveUnitZoneData>();
        }
    }
}