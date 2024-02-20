using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System.Collections.Generic;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.AI.Arena;
using CyberNet.Core.Arena;
using CyberNet.Core.City;
using CyberNet.Core.Player;
using CyberNet.Global;
using CyberNet.Global.Cursor;
using DG.Tweening;

namespace CyberNet.Core.Map
{
    [EcsSystem(typeof(CoreModule))]
    public class MapMoveUnitsSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            MapMoveUnitsAction.StartMoveUnits += StartMoveUnit;
            MapMoveUnitsAction.StartArenaBattle += StartArenaBattle;
        }
        
        private void StartMoveUnit()
        {
            // Сейчас зону атаки выбирает верно, но не выбирает юнитов для атаки
            var entityMoveCard = _dataWorld.Select<AbilityCardMoveUnitComponent>().SelectFirstEntity();
            var selectUnitEntities = _dataWorld.Select<SelectUnitMapComponent>().GetEntities();
            var selectTowerForAttackGuid = entityMoveCard.GetComponent<AbilityCardMoveUnitComponent>().SelectTowerGUID;
            
            var targetTowerEntity = _dataWorld.Select<TowerComponent>()
                .Where<TowerComponent>(tower => tower.GUID == selectTowerForAttackGuid)
                .SelectFirstEntity();
            var targetTowerComponent = targetTowerEntity.GetComponent<TowerComponent>();
            
            var countUnitEntityIsDefensePosition = _dataWorld.Select<UnitMapComponent>()
                .Where<UnitMapComponent>(unit => unit.GUIDTower == selectTowerForAttackGuid).Count();

            var targetSlotZone = 0;
            if (countUnitEntityIsDefensePosition > 0)
            {
                var entitiesUnitEntityIsDefensePosition = _dataWorld.Select<UnitMapComponent>()
                    .Where<UnitMapComponent>(unit => unit.GUIDTower == selectTowerForAttackGuid)
                    .GetEntities();
                
                foreach (var entity in entitiesUnitEntityIsDefensePosition)
                {
                    entity.AddComponent(new UnitInBattleArenaComponent {
                        Forwards = false
                    });
                }
                
                foreach (var entity in selectUnitEntities)
                {
                    entity.AddComponent(new UnitInBattleArenaComponent {
                        Forwards = true
                    });
                }

                targetSlotZone = GetEnemySlotInTargetZone(selectTowerForAttackGuid);
            }
            else
            {
                targetSlotZone = GetFriendlySlotInTargetZone(selectTowerForAttackGuid);
            }
            
            var positionAllPoint = new List<Vector3>();
            foreach (var unitEntity in selectUnitEntities)
            {
                unitEntity.RemoveComponent<SelectUnitMapComponent>();
                ref var unitComponent = ref unitEntity.GetComponent<UnitMapComponent>();
                
                var targetPosition = CitySupportStatic.SelectPosition
                (
                    targetTowerComponent.SquadZonesMono[targetSlotZone].Collider,
                    targetTowerComponent.SquadZonesMono[targetSlotZone].transform.position,
                    positionAllPoint
                );
                positionAllPoint.Add(targetPosition);
                
                unitEntity.AddComponent(new MoveUnitToTargetComponent {
                    TargetPosition = targetPosition,
                    TargetTowerGUID = selectTowerForAttackGuid,
                    TargetSlotID = targetSlotZone
                });

                unitComponent.GUIDTower = selectTowerForAttackGuid;
                unitComponent.IndexPoint = targetSlotZone;
                
                unitComponent.IconsUnitInMapMono.OffSelectUnitEffect();
                unitComponent.UnitIconsGO.transform.SetParent(targetTowerComponent.SquadZonesMono[targetSlotZone].transform);
            }
            
            AnimationMoveToTarget();
        }

        private int GetFriendlySlotInTargetZone(string towerGUID)
        {
            var currentPlayerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();
            var currentPlayerComponent = currentPlayerEntity.GetComponent<PlayerComponent>();
            
            var towerEntity = _dataWorld.Select<TowerComponent>()
                .Where<TowerComponent>(tower => tower.GUID == towerGUID)
                .SelectFirstEntity();
            ref var towerComponent = ref towerEntity.GetComponent<TowerComponent>();
            
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
            
            var towerEntity = _dataWorld.Select<TowerComponent>()
                .Where<TowerComponent>(tower => tower.GUID == towerGUID)
                .SelectFirstEntity();
            ref var towerComponent = ref towerEntity.GetComponent<TowerComponent>();
            
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

                unitEntity.AddComponent(new TimeComponent {
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
                var playerType = _dataWorld.OneData<RoundData>().playerOrAI;

                if (playerType != PlayerOrAI.Player)
                {
                    AIBattleArenaAction.CheckEnemyBattle?.Invoke();
                }
                else
                {
                    StartArenaBattle();
                }
            }
        }

        private void StartArenaBattle()
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();
            roundData.CurrentRoundState = RoundState.Arena;
            ref var arenaData = ref _dataWorld.OneData<ArenaData>();
            arenaData.IsShowVisualBattle = true;
            
            CustomCursorAction.OnBaseCursor?.Invoke();
            ZoomCameraToBattle();
        }

        private void ZoomCameraToBattle()
        {
            AnimationsStartArenaCameraAction.StartAnimations?.Invoke(0.75f);

            var entity = _dataWorld.NewEntity();
            entity.AddComponent(new TimeComponent {
                Time = 0.75f,
                Action = () => {
                    entity.Destroy();
                    FinishAnimation();
                }
            });
        }

        private void FinishAnimation()
        {
            ArenaAction.StartArenaBattle?.Invoke();
        }

        public void Destroy()
        {
            MapMoveUnitsAction.StartMoveUnits -= StartMoveUnit;
            MapMoveUnitsAction.StartArenaBattle -= StartArenaBattle;
        }
    }
}