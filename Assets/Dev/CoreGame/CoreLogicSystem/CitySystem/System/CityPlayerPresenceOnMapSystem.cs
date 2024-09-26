using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using System.Collections.Generic;
using CyberNet.Core.Player;
using CyberNet.Core.UI;

namespace CyberNet.Core.Map
{
    [EcsSystem(typeof(CoreModule))]
    public class CityPlayerPresenceOnMapSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            CityAction.UpdatePresencePlayerInCity += UpdatePresencePlayerInCity;
        }
        
        private void UpdatePresencePlayerInCity()
        {
            ClearOldPresencePlayerComponent();
            UpdatePlayerControlTower();
            AddComponentPresencePlayer();
            UpdateCountControlTowerPlayer();
            
            CityAction.UpdateTowerControlView?.Invoke();
            BoardGameUIAction.UpdateStatsAllPlayersPassportUI?.Invoke();
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
                towerMono.OffInteractiveTower();
                towerMono.CloseInteractiveZoneVisualEffect();
            }
        }

        private void UpdatePlayerControlTower()
        {
            var towerEntities = _dataWorld.Select<TowerComponent>()
                .GetEntities();
            
            //Debug.LogError("update player control tower");
            foreach (var towerEntity in towerEntities)
            {
                ref var towerComponent = ref towerEntity.GetComponent<TowerComponent>();
                var towerGUID = towerComponent.GUID;

                var unitInTowerEntities = _dataWorld.Select<UnitMapComponent>()
                    .Where<UnitMapComponent>(unit => unit.GUIDTower == towerGUID)
                    .GetEntities();
                
                var playersInTower = new List<int>();
                
                foreach (var unitEntity in unitInTowerEntities)
                {
                    //Debug.LogError($"Check unit in map tower {towerComponent.Key}");
                    var unitComponent = unitEntity.GetComponent<UnitMapComponent>();
                    var isDouble = false;
                    
                    foreach (var playerID in playersInTower)
                    {
                        if (playerID == unitComponent.PowerSolidPlayerID)
                        {
                            isDouble = true;
                            break;
                        }
                    }

                    if (!isDouble)
                    {
                        playersInTower.Add(unitComponent.PowerSolidPlayerID);
                    }
                }
                
                var maxUnit = 0;
                var IDPlayerControlTower = -10;

                foreach (var playerID in playersInTower)
                {
                    var countCurrentPlayerUnit = _dataWorld.Select<UnitMapComponent>()
                        .Where<UnitMapComponent>(unit => unit.GUIDTower == towerGUID
                        && unit.PowerSolidPlayerID == playerID)
                        .Count();

                    //Debug.LogError($"кол-во юнитов {countCurrentPlayerUnit} принадлежит данному игроку {playerID} в районе {towerComponent.Key}");
                    
                    if (countCurrentPlayerUnit > maxUnit)
                        IDPlayerControlTower = playerID;
                }

                if (IDPlayerControlTower == -10)
                {
                    //Debug.LogError($"район никому не принадлежит {towerComponent.Key}");
                    continue;
                }
                
                if (IDPlayerControlTower == -1)
                {
                    //Debug.LogError($"Район нейтральный {towerComponent.Key}");
                    towerComponent.TowerBelongPlayerID = IDPlayerControlTower;
                    towerComponent.PlayerControlEntity = PlayerControlEntity.NeutralUnits;
                }
                else
                {
                    //Debug.LogError($"Район принадлежит {IDPlayerControlTower}");
                    towerComponent.TowerBelongPlayerID = IDPlayerControlTower;
                    towerComponent.PlayerControlEntity = PlayerControlEntity.PlayerControl;
                }
            }
        }

        public void AddComponentPresencePlayer()
        {
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;

            var unitEntities = _dataWorld.Select<UnitMapComponent>()
                .Where<UnitMapComponent>(unit => unit.PowerSolidPlayerID == currentPlayerID)
                .GetEntities();

            var isFindGUIDTower = new List<string>();            
            foreach (var unitEntity in unitEntities)
            {
                var unitComponent = unitEntity.GetComponent<UnitMapComponent>();

                var isDouble = false;
                foreach (var findGUID in isFindGUIDTower)
                {
                    if (findGUID == unitComponent.GUIDTower)
                        isDouble = true;
                }

                if (!isDouble)
                {
                    AddPresenceComponent(unitComponent.GUIDTower);
                    isFindGUIDTower.Add(unitComponent.GUIDTower);
                }
            }
        }

        private void AddPresenceComponent(string guidPoint)
        {
            var towerEntity = _dataWorld.Select<TowerComponent>()
                .Where<TowerComponent>(tower => tower.GUID == guidPoint)
                .SelectFirstEntity();

            towerEntity.AddComponent(new PresencePlayerTowerComponent());
            CityAction.UpdateTowerControlView?.Invoke();
        }
        
        private void UpdateCountControlTowerPlayer()
        {
            var playerEntities = _dataWorld.Select<PlayerComponent>().GetEntities();

            foreach (var playerEntity in playerEntities)
            {
                ref var playerComponent = ref playerEntity.GetComponent<PlayerComponent>();
                var playerID = playerComponent.PlayerID;
                var countControlTerritoryPlayer = _dataWorld.Select<TowerComponent>()
                    .Where<TowerComponent>(tower => tower.TowerBelongPlayerID == playerID
                        && tower.PlayerControlEntity == PlayerControlEntity.PlayerControl)
                    .Count();

                playerComponent.VictoryPoint = countControlTerritoryPlayer;
            }
        }

        public void Destroy()
        {
            CityAction.UpdatePresencePlayerInCity -= UpdatePresencePlayerInCity;
        }
    }
}