using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using System.Collections.Generic;
using UnityEngine;

namespace CyberNet.Core.City
{
    [EcsSystem(typeof(CoreModule))]
    public class CityGameMechanicsSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            CityAction.UpdatePresencePlayerInCity += UpdatePresencePlayerInCity;
            CityAction.EnableNewPresencePlayerInCity += EnableAllPresencePlayerPoint;
        }

        public void UpdatePresencePlayerInCity()
        {
            ClearOldPresencePlayerComponent();
            AddComponentPresencePlayer();
            EnableAllPresencePlayerPoint();
        }

        public void AddComponentPresencePlayer()
        {
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;

            var unitEntities = _dataWorld.Select<SquadMapComponent>()
                .Where<SquadMapComponent>(unit => unit.PowerSolidPlayerID == currentPlayerID)
                .GetEntities();

            var isFindGUID = new List<string>();            
            foreach (var unitEntity in unitEntities)
            {
                var unitComponent = unitEntity.GetComponent<SquadMapComponent>();

                var isDouble = false;
                foreach (var findGUID in isFindGUID)
                {
                    if (findGUID == unitComponent.GUIDPoint)
                        isDouble = true;
                }

                if (!isDouble)
                {
                    FindUnitContainer(unitComponent.GUIDPoint);
                    isFindGUID.Add(unitComponent.GUIDPoint);
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
        }

        private void FindNeighboringTowerPoint(string towerGUID)
        {
            /*
            var connectPointEntities = _dataWorld.Select<ConnectPointComponent>().GetEntities();

            foreach (var connectPointEntity in connectPointEntities)
            {
                ref var connectPointComponent = ref connectPointEntity.GetComponent<ConnectPointComponent>();

                foreach (var connectPointTypeGuid in connectPointComponent.ConnectPointsTypeGUID)
                {
                    if (connectPointTypeGuid.GUIDPoint == towerGUID)
                        connectPointEntity.AddComponent(new PresencePlayerPointCityComponent());
                }
            }*/
        }

        private void ClearOldPresencePlayerComponent()
        {
            var towerEntities = _dataWorld.Select<TowerComponent>().GetEntities();

            foreach (var towerEntity in towerEntities)
            {
                towerEntity.RemoveComponent<PresencePlayerPointCityComponent>();
            }
        }
        
        private void EnableAllPresencePlayerPoint()
        {
            var towerEntities = _dataWorld.Select<TowerComponent>()
                .With<PresencePlayerPointCityComponent>()
                .GetEntities();
        }
    }
}