using System.Collections.Generic;
using CyberNet.Core.City;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace CyberNet.Core.AI
{
    [EcsSystem(typeof(CoreModule))]
    public class EnemyAISupportSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            EnemyAISupportAction.GetTowerFreeSlotPlayerPresence += GetTowerFreeSlotPlayerPresence;
            EnemyAISupportAction.GetConnectPointFreeSlotPlayerPresence += GetConnectPointFreeSlotPlayerPresence;
            EnemyAISupportAction.CheckEnemyPresenceInTower += CheckEnemyPresenceInTower;
        }

        private List<BuildFreeSlotStruct> GetTowerFreeSlotPlayerPresence()
        {
            var towerEntities = _dataWorld.Select<TowerComponent>()
                .With<PresencePlayerPointCityComponent>()
                .GetEntities();

            var buildFreeSlot = new List<BuildFreeSlotStruct>();
            
            foreach (var towerEntity in towerEntities)
            {
                var towerComponent = towerEntity.GetComponent<TowerComponent>();
                var unitCountInTower  = _dataWorld.Select<UnitComponent>()
                    .Where<UnitComponent>(unit => unit.GUIDPoint == towerComponent.GUID)
                    .Count();
                
                var unitInTowerEntities  = _dataWorld.Select<UnitComponent>()
                    .Where<UnitComponent>(unit => unit.GUIDPoint == towerComponent.GUID)
                    .GetEntities();
                
                if (towerComponent.SolidPointMono.Count > unitCountInTower)
                {
                    var buildSlot = new BuildFreeSlotStruct {
                        CountFreeSlot = towerComponent.SolidPointMono.Count - unitCountInTower,
                        GUID = towerComponent.GUID,
                        SolidPointsMono = new()
                    };

                    foreach (var solidPoint in towerComponent.SolidPointMono)
                    {
                        var isClose = false;

                        foreach (var unitInTowerEntity in unitInTowerEntities)
                        {
                            ref var unitComponent = ref unitInTowerEntity.GetComponent<UnitComponent>();
                            if (solidPoint.Index == unitComponent.IndexPoint)
                            {
                                isClose = true;
                                break;
                            }
                        }
                        
                        if (!isClose)
                            buildSlot.SolidPointsMono.Add(solidPoint);
                    }
                    
                    buildFreeSlot.Add(buildSlot);
                }
            }

            return buildFreeSlot;
        }
        
        private List<BuildFreeSlotStruct> GetConnectPointFreeSlotPlayerPresence()
        {
            var connectPointEntities = _dataWorld.Select<ConnectPointComponent>()
                .With<PresencePlayerPointCityComponent>()
                .GetEntities();

            var buildFreeSlot = new List<BuildFreeSlotStruct>();
            
            foreach (var connectPointEntity in connectPointEntities)
            {
                var connectPointComponent = connectPointEntity.GetComponent<ConnectPointComponent>();
                var unitCountInConnectPoint = _dataWorld.Select<UnitComponent>()
                    .Where<UnitComponent>(unit => unit.GUIDPoint == connectPointComponent.GUID)
                    .Count();
                
                if (unitCountInConnectPoint == 0)
                {
                    var buildSlot = new BuildFreeSlotStruct {
                        CountFreeSlot = 1,
                        GUID = connectPointComponent.GUID,
                        SolidPointsMono = new List<SolidPointMono>()
                    };
                    buildSlot.SolidPointsMono.Add(connectPointComponent.SolidPointMono);
                    buildFreeSlot.Add(buildSlot);
                }
            }
            
            return buildFreeSlot;
        }

        public List<EnemyInTowerLink> CheckEnemyPresenceInTower()
        {
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;
            var towerEntities = _dataWorld.Select<TowerComponent>()
                .With<PresencePlayerPointCityComponent>()
                .GetEntities();
            var enemyInTower = new List<EnemyInTowerLink>();

            foreach (var towerEntity in towerEntities)
            {
                var towerComponent = towerEntity.GetComponent<TowerComponent>();
                var enemyUnitEntities = _dataWorld.Select<UnitComponent>()
                    .Where<UnitComponent>(unit => unit.GUIDPoint == towerComponent.GUID && unit.PowerSolidPlayerID != currentPlayerID)
                    .GetEntities();

                foreach (var unitEntity in enemyUnitEntities)
                {
                    ref var unitComponent = ref unitEntity.GetComponent<UnitComponent>();
                    enemyInTower.Add(new EnemyInTowerLink {
                        Index = unitComponent.IndexPoint,
                        GUID = unitComponent.GUIDPoint
                    });
                }

            }

            return enemyInTower;
        }

        public void SelectTargetAttack()
        {
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;
            var towerEntities = _dataWorld.Select<TowerComponent>()
                .Where<TowerComponent>(tower => tower.TowerBelongPlyaerID != currentPlayerID)
                .GetEntities();
            
            
        }
    }
}