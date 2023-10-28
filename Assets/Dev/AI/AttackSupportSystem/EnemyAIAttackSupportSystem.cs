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
    public class EnemyAIAttackSupportSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            EnemyAIAttackSupportAction.GetTowerFreeSlotPlayerPresence += GetTowerFreeSlotPlayerPresence;
            EnemyAIAttackSupportAction.CheckEnemyPresenceInBuild += CheckEnemyPresenceInBuild;
            EnemyAIAttackSupportAction.SelectTargetAttack += SelectTargetAttack;
        }

        private List<BuildFreeSlotStruct> GetTowerFreeSlotPlayerPresence()
        {
            var towerEntities = _dataWorld.Select<TowerComponent>()
                .With<PresencePlayerTowerComponent>()
                .GetEntities();

            var buildFreeSlot = new List<BuildFreeSlotStruct>();
            
            foreach (var towerEntity in towerEntities)
            {
                var towerComponent = towerEntity.GetComponent<TowerComponent>();
                var unitCountInTower  = _dataWorld.Select<SquadMapComponent>()
                    .Where<SquadMapComponent>(unit => unit.GUIDPoint == towerComponent.GUID)
                    .Count();
                
                var unitInTowerEntities  = _dataWorld.Select<SquadMapComponent>()
                    .Where<SquadMapComponent>(unit => unit.GUIDPoint == towerComponent.GUID)
                    .GetEntities();
                
                if (towerComponent.SquadZonesMono.Count > unitCountInTower)
                {
                    var buildSlot = new BuildFreeSlotStruct {
                        CountFreeSlot = towerComponent.SquadZonesMono.Count - unitCountInTower,
                        GUID = towerComponent.GUID,
                        SolidPointsMono = new()
                    };

                    foreach (var solidPoint in towerComponent.SquadZonesMono)
                    {
                        var isClose = false;

                        foreach (var unitInTowerEntity in unitInTowerEntities)
                        {
                            ref var unitComponent = ref unitInTowerEntity.GetComponent<SquadMapComponent>();
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
        
        public List<EnemyInBuildLink> CheckEnemyPresenceInBuild()
        {
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;
            var enemyInBuild = new List<EnemyInBuildLink>();

            var towerEntities = _dataWorld.Select<TowerComponent>()
                .With<PresencePlayerTowerComponent>()
                .GetEntities();
            
            foreach (var towerEntity in towerEntities)
            {
                var towerComponent = towerEntity.GetComponent<TowerComponent>();
                var enemyUnitEntities = _dataWorld.Select<SquadMapComponent>()
                    .Where<SquadMapComponent>(unit => unit.GUIDPoint == towerComponent.GUID && unit.PowerSolidPlayerID != currentPlayerID)
                    .GetEntities();

                foreach (var unitEntity in enemyUnitEntities)
                {
                    ref var unitComponent = ref unitEntity.GetComponent<SquadMapComponent>();
                    enemyInBuild.Add(new EnemyInBuildLink {
                        Index = unitComponent.IndexPoint,
                        GUID = unitComponent.GUIDPoint,
                    });
                }
            }

            return enemyInBuild;
        }

        public string SelectTargetAttack()
        {
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;
            var towerEntities = _dataWorld.Select<TowerComponent>()
                .Where<TowerComponent>(tower => tower.TowerBelongPlyaerID != currentPlayerID)
                .GetEntities();

            ref var towerConfigDict = ref _dataWorld.OneData<BoardGameData>().TowerConfig;

            var guidTowerMaxEfficiency = "";
            var towerMaxEfficiency = 0;
            
            foreach (var towerEntity in towerEntities)
            {
                if (towerEntity.HasComponent<PresencePlayerTowerComponent>())
                    continue;
                
                ref var towerComponent = ref towerEntity.GetComponent<TowerComponent>();
                towerConfigDict.TryGetValue(towerComponent.Key, out var towerConfig);
                
                var efficiencyCurrentTower = towerConfig.RewardEndGame.Value;
                
                if (towerConfig.RewardControl != null)
                    efficiencyCurrentTower = efficiencyCurrentTower * 2;

                var minPatchTower = CalculatePatchTower(towerComponent.GUID);
                efficiencyCurrentTower -= minPatchTower;
                
                if (efficiencyCurrentTower > towerMaxEfficiency)
                {
                    guidTowerMaxEfficiency = towerComponent.GUID;
                    towerMaxEfficiency = efficiencyCurrentTower;
                }
            }

            return guidTowerMaxEfficiency;
        }

        private int CalculatePatchTower(string guid)
        {
            var value = Random.Range(2, 6);
            
            return value;
        }
    }
}