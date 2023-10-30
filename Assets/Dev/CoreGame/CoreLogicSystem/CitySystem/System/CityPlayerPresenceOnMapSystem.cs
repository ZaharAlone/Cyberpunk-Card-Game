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
    public class CityPlayerPresenceOnMapSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            CityAction.UpdatePresencePlayerInCity += UpdatePresencePlayerInCity;
        }
        
        public void UpdatePresencePlayerInCity()
        {
            ClearOldPresencePlayerComponent();
            AddComponentPresencePlayer();
        }
        
        private void ClearOldPresencePlayerComponent()
        {
            var towerEntities = _dataWorld.Select<TowerComponent>()
                .With<PresencePlayerTowerComponent>()
                .GetEntities();

            foreach (var towerEntity in towerEntities)
            {
                towerEntity.RemoveComponent<PresencePlayerTowerComponent>();
                var towerMono = towerEntity.GetComponent<TowerComponent>().TowerMono;
                towerMono.DeactivateCollider();
                towerMono.CloseInteractiveZoneVisualEffect();
            }
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
                    AddPresenceComponent(unitComponent.GUIDPoint);
                    isFindGUID.Add(unitComponent.GUIDPoint);
                }
            }
        }

        private void AddPresenceComponent(string guidUnit)
        {
            var towerEntity = _dataWorld.Select<TowerComponent>()
                .Where<TowerComponent>(tower => tower.GUID == guidUnit)
                .SelectFirstEntity();

            towerEntity.AddComponent(new PresencePlayerTowerComponent());
            CityAction.UpdatePlayerViewCity?.Invoke();
        }
    }
}