using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace CyberNet.Core.City
{
    [EcsSystem(typeof(CoreModule))]
    public class CityGameMechanicsSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            RoundAction.StartTurn += UpdatePresencePlayerInCity;
            CityAction.UpdatePresencePlayerInCity += UpdatePresencePlayerInCity;
            CityAction.EnableNewPresencePlayerInCity += EnableAllPresencePlayerPoint;
        }

        public void UpdatePresencePlayerInCity()
        {
            ClearOldPresencePlayerComponent();
            AddComponentPresencePlayerInTower();
            EnableAllPresencePlayerPoint();
        }

        public void AddComponentPresencePlayerInTower()
        {
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;

            var unitEntities = _dataWorld.Select<UnitComponent>()
                .Where<UnitComponent>(unit => unit.PowerSolidPlayerID == currentPlayerID)
                .GetEntities();

            var findTowerGUID = new List<string>();            
            foreach (var unitEntity in unitEntities)
            {
                var unitComponent = unitEntity.GetComponent<UnitComponent>();

                var isDouble = false;
                foreach (var findGUID in findTowerGUID)
                {
                    if (findGUID == unitComponent.GUIDPoint)
                        isDouble = true;
                }

                if (!isDouble)
                {
                    FindUnitContainer(unitComponent.GUIDPoint);
                    findTowerGUID.Add(unitComponent.GUIDPoint);
                }
            }
        }

        private void FindUnitContainer(string guidUnit)
        {
            var isFindTower = _dataWorld.Select<TowerComponent>()
                .Where<TowerComponent>(tower => tower.GUID == guidUnit)
                .TrySelectFirstEntity(out var towerEntity);

            if (isFindTower)
            {
                towerEntity.AddComponent(new PresencePlayerPointCityComponent());
                FindNeighboringTowerPoint(guidUnit);
            }
            else
            {
                var connectPointEntity = _dataWorld.Select<ConnectPointComponent>()
                    .Where<ConnectPointComponent>(connectPoint => connectPoint.GUID == guidUnit)
                    .SelectFirstEntity();

                connectPointEntity.AddComponent(new PresencePlayerPointCityComponent());

                ref var connectPointComponent = ref connectPointEntity.GetComponent<ConnectPointComponent>();
                FindNeighboringConnectPoint(connectPointComponent.ConnectPointsTypeGUID);
            }
        }

        private void FindNeighboringTowerPoint(string towerGUID)
        {
            var connectPointEntities = _dataWorld.Select<ConnectPointComponent>().GetEntities();

            foreach (var connectPointEntity in connectPointEntities)
            {
                ref var connectPointComponent = ref connectPointEntity.GetComponent<ConnectPointComponent>();

                foreach (var connectPointTypeGuid in connectPointComponent.ConnectPointsTypeGUID)
                {
                    if (connectPointTypeGuid.GUIDPoint == towerGUID)
                        connectPointEntity.AddComponent(new PresencePlayerPointCityComponent());
                }
            }
        }
        
        private void FindNeighboringConnectPoint(List<ConnectPointTypeGUID> connectPoints)
        {
            foreach (var connectPoint in connectPoints)
            {
                if (connectPoint.TypeCityPoint == TypeCityPoint.Tower)
                {
                    var towerEntity = _dataWorld.Select<TowerComponent>()
                        .Where<TowerComponent>(tower => tower.GUID == connectPoint.GUIDPoint)
                        .SelectFirstEntity();

                    towerEntity.AddComponent(new PresencePlayerPointCityComponent());
                }
                else
                {
                    var connectPointEntity = _dataWorld.Select<ConnectPointComponent>()
                        .Where<ConnectPointComponent>(connectPoint => connectPoint.GUID == connectPoint.GUID)
                        .SelectFirstEntity();

                    connectPointEntity.AddComponent(new PresencePlayerPointCityComponent());
                }
            }
        }

        private void ClearOldPresencePlayerComponent()
        {
            var towerEntities = _dataWorld.Select<TowerComponent>().GetEntities();

            foreach (var towerEntity in towerEntities)
            {
                towerEntity.RemoveComponent<PresencePlayerPointCityComponent>();
                ref var towerComponent = ref towerEntity.GetComponent<TowerComponent>();
                towerComponent.TowerMono.DeactivateSolidPointCollider();
            }
            
            var connectPointEntities = _dataWorld.Select<ConnectPointComponent>().GetEntities();

            foreach (var connectPointEntity in connectPointEntities)
            {
                connectPointEntity.RemoveComponent<PresencePlayerPointCityComponent>();
                ref var connectPointComponent = ref connectPointEntity.GetComponent<ConnectPointComponent>();
                connectPointComponent.ConnectPointMono.DeactivateSolidPointCollider();
            }
        }
        
        private void EnableAllPresencePlayerPoint()
        {
            var towerEntities = _dataWorld.Select<TowerComponent>()
                .With<PresencePlayerPointCityComponent>()
                .GetEntities();

            foreach (var towerEntity in towerEntities)
            {
                ref var towerComponent = ref towerEntity.GetComponent<TowerComponent>();
                towerComponent.TowerMono.ActivateSolidPointCollider();
            }
            
            var connectPointEntities = _dataWorld.Select<ConnectPointComponent>()
                .With<PresencePlayerPointCityComponent>()
                .GetEntities();

            foreach (var connectPointEntity in connectPointEntities)
            {
                ref var connectPointComponent = ref connectPointEntity.GetComponent<ConnectPointComponent>();
                connectPointComponent.ConnectPointMono.ActivateSolidPointCollider();
            }
        }
    }
}