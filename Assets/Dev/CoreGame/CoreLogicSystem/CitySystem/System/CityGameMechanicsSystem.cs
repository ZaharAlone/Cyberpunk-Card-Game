using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using System.Collections.Generic;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.Player;
using UnityEngine;

namespace CyberNet.Core.City
{
    [EcsSystem(typeof(CoreModule))]
    public class CityGameMechanicsSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            CityAction.UpdateCanInteractiveMap += UpdateCanInteractiveMap;
            CityAction.ShowWherePlayerCanMove += ShowWherePlayerCanMove;
            CityAction.ShowWherePlayerCanMoveFrom += ShowWherePlayerCanMoveFrom;
            CityAction.ShowWhereZoneToPlayerID += ShowWhereZoneNeutralUnit;
            CityAction.ShowManyZonePlayerInMap += ShowManyZonePlayerInMap;
        }
        
        private void UpdateCanInteractiveMap()
        {
            var towerEntities = _dataWorld.Select<TowerComponent>().GetEntities();
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();
            var playerComponent = playerEntity.GetComponent<PlayerComponent>();

            foreach (var towerEntity in towerEntities)
            {
                var towerComponent = towerEntity.GetComponent<TowerComponent>();

                if (towerComponent.PlayerControlEntity == PlayerControlEntity.PlayerControl
                    && towerComponent.TowerBelongPlayerID == playerComponent.PlayerID)
                {
                    towerComponent.TowerMono.ActivateCollider();
                    towerComponent.TowerMono.OpenInteractiveZoneVisualEffect();
                }
                else
                {
                    towerComponent.TowerMono.DeactivateCollider();
                    towerComponent.TowerMono.CloseInteractiveZoneVisualEffect();
                }
            }
        }
        
        /// <summary>
        /// Активирует зоны на которые игрок может передвинуть своего юнита
        /// </summary>
        private void ShowWherePlayerCanMove()
        {
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();

            var playerComponent = playerEntity.GetComponent<PlayerComponent>();

            var towerEntities = _dataWorld.Select<TowerComponent>()
                .Where<TowerComponent>(tower => tower.PlayerControlEntity == PlayerControlEntity.PlayerControl
                    && tower.TowerBelongPlayerID == playerComponent.PlayerID)
                .GetEntities();
            
            var countTowerEntity = _dataWorld.Select<TowerComponent>()
                .Where<TowerComponent>(tower => tower.PlayerControlEntity == PlayerControlEntity.PlayerControl
                    && tower.TowerBelongPlayerID == playerComponent.PlayerID)
                .Count();

            foreach (var towerEntity in towerEntities)
            {
                var towerComponent = towerEntity.GetComponent<TowerComponent>();

                if (countTowerEntity > 1)
                {
                    towerComponent.TowerMono.ActivateCollider();
                    towerComponent.TowerMono.OpenInteractiveZoneVisualEffect();   
                }

                foreach (var towerConnect in towerComponent.TowerMono.ZoneConnect)
                {
                    towerConnect.ActivateCollider();
                    towerConnect.OpenInteractiveZoneVisualEffect();
                }
            }
        }
        
        /// <summary>
        /// Активирует зоны с которых игрок может передвинуть своих юнитов в целевую точку
        /// </summary>
        private void ShowWherePlayerCanMoveFrom(string GUIDTower)
        {
            DeactivateAllTower();
            
            var towerEntity = _dataWorld.Select<TowerComponent>()
                .Where<TowerComponent>(tower => tower.GUID == GUIDTower)
                .SelectFirstEntity();

            var towerComponent = towerEntity.GetComponent<TowerComponent>();

            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();

            var playerComponent = playerEntity.GetComponent<PlayerComponent>();
            foreach (var towerConnect in towerComponent.TowerMono.ZoneConnect)
            {
                var towerConnectEntity = _dataWorld.Select<TowerComponent>()
                    .Where<TowerComponent>(tower => tower.GUID == towerConnect.GUID)
                    .SelectFirstEntity();

                var towerConnectComponent = towerConnectEntity.GetComponent<TowerComponent>();
                if (towerConnectComponent.PlayerControlEntity == PlayerControlEntity.PlayerControl
                    && towerConnectComponent.TowerBelongPlayerID == playerComponent.PlayerID)
                {
                    towerConnectComponent.TowerMono.OpenInteractiveZoneVisualEffect();
                }
            }
        }

        private void DeactivateAllTower()
        {
            var towerEntities = _dataWorld.Select<TowerComponent>().GetEntities();
            foreach (var towerEntity in towerEntities)
            {
                var towerComponent = towerEntity.GetComponent<TowerComponent>();
                towerComponent.TowerMono.DeactivateCollider();
                towerComponent.TowerMono.CloseInteractiveZoneVisualEffect();
            }
        }
        
        private void ShowWhereZoneNeutralUnit(int targetPlayerID) // Select Type zone
        {
            DeactivateAllTower();
            SelectZoneOneUnit(targetPlayerID);
        }

        private void ShowManyZonePlayerInMap(List<int> targetPlayerID)
        {
            DeactivateAllTower();

            foreach (var playerID in targetPlayerID)
            {
                SelectZoneOneUnit(playerID);
            }
        }

        private void SelectZoneOneUnit(int targetPlayerID)
        {
            var unitEntities = _dataWorld.Select<UnitMapComponent>()
                .Where<UnitMapComponent>(unit => unit.PowerSolidPlayerID == targetPlayerID)
                .GetEntities();

            var uniqueTowerGUID = new List<string>();
            foreach (var unitEntity in unitEntities)
            {
                var unitComponent = unitEntity.GetComponent<UnitMapComponent>();
                var isDouble = false;

                foreach (var towerGuid in uniqueTowerGUID)
                {
                    if (towerGuid == unitComponent.GUIDTower)
                    {
                        isDouble = true;
                        break;
                    }
                }
                
                if (!isDouble)
                    uniqueTowerGUID.Add(unitComponent.GUIDTower);
            }

            foreach (var towerGUID in uniqueTowerGUID)
            {
                var towerEntity = _dataWorld.Select<TowerComponent>()
                    .Where<TowerComponent>(tower => tower.GUID == towerGUID)
                    .SelectFirstEntity();
                
                var towerComponent = towerEntity.GetComponent<TowerComponent>();
                towerComponent.TowerMono.ActivateCollider();
                towerComponent.TowerMono.OpenInteractiveZoneVisualEffect();
            }
        }
    }
}