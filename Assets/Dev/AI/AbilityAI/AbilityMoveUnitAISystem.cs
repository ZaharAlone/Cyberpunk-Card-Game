using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using System.Collections.Generic;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.City;
using CyberNet.Core.Player;

namespace CyberNet.Core.AI
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityMoveUnitAISystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AbilityAIAction.MoveUnit += MoveUnit;
        }
        
        private void MoveUnit(string guidCard)
        {
            var entityCard = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == guidCard)
                .SelectFirstEntity();
            
            //var cardComponent = entityCard.GetComponent<CardComponent>();
            //var abilitySelectElementComponent = entityCard.GetComponent<AbilitySelectElementComponent>();
            
            Debug.LogError("Move unit AI");
        }
        /*
        private void FindOptimalTowerForMove()
        {
            var currentPlayerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();

            var currentPlayerComponent = currentPlayerEntity.GetComponent<PlayerComponent>();
            var towerEntities = _dataWorld.Select<TowerComponent>()
                .Where<TowerComponent>(tower => tower.TowerBelongPlyaerID == currentPlayerComponent.PlayerID
                    && tower.PlayerIsBelong == PlayerControlEnum.Player)
                .GetEntities();
            
            // 
            
            var guidSelectPotentiallyWeakTower = new List<string>();
            foreach (var towerEntity in towerEntities)
            {
                var towerComponent = towerEntity.GetComponent<TowerComponent>();
                
                foreach (var zoneConnectTower in towerComponent.TowerMono.ZoneConnect)
                {
                    var connectTowerEntity = _dataWorld.Select<TowerComponent>()
                        .Where<TowerComponent>(tower => tower.GUID == zoneConnectTower.GUID)
                        .SelectFirstEntity();
                    var connectTowerComponent = connectTowerEntity.GetComponent<TowerComponent>();

                    if (connectTowerComponent.TowerBelongPlyaerID != currentPlayerComponent.PlayerID
                        && connectTowerComponent.PlayerIsBelong == PlayerControlEnum.Player)
                    {
                        guidSelectPotentiallyWeakTower.Add(towerComponent.GUID);
                        break;
                    }
                }
            }
            
            if (guidSelectPotentiallyWeakTower.Count == 0)
            {
                guidSelectPotentiallyWeakTower.Clear();
                var maxConnectZone = 0;
                foreach (var towerEntity in towerEntities)
                {
                    var towerComponent = towerEntity.GetComponent<TowerComponent>();

                    var countConnectZone = 0;
                    foreach (var zoneConnectTower in towerComponent.TowerMono.ZoneConnect)
                    {
                        var connectTowerEntity = _dataWorld.Select<TowerComponent>()
                            .Where<TowerComponent>(tower => tower.GUID == zoneConnectTower.GUID)
                            .SelectFirstEntity();
                        var connectTowerComponent = connectTowerEntity.GetComponent<TowerComponent>();
        
                        if (connectTowerComponent.TowerBelongPlyaerID != currentPlayerComponent.PlayerID)
                        {
                            countConnectZone++;
                        }
                    }

                    if (countConnectZone > maxConnectZone)
                    {
                        guidSelectPotentiallyWeakTower.Add(towerComponent.GUID);
                        maxConnectZone = countConnectZone;
                    }
                }
            }
        }

        private string FindZoneMinUnit(List<string> selectTower, int playerID)
        {
            var selectGUID = "";
            var minCountUnit = 99;
            
            foreach (var towerGUID in selectTower)
            {
                var countUnit = _dataWorld.Select<UnitMapComponent>()
                    .Where<UnitMapComponent>(unit => unit.GUIDTower == towerGUID
                        && unit.PowerSolidPlayerID == playerID)
                    .Count();

                if (countUnit < minCountUnit)
                {
                    selectGUID = towerGUID;
                    minCountUnit = countUnit;
                }
            }

            return selectGUID;
        }
*/
        public void Destroy()
        {
            
        }
    }
}