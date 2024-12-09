using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using System.Collections.Generic;
using CyberNet.Core.Player;

namespace CyberNet.Core.Map
{
    [EcsSystem(typeof(CoreModule))]
    public class CityGameMechanicsSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            CityAction.UpdateCanInteractiveMap += UpdateCanInteractiveMap;
            CityAction.ShowWherePlayerCanMove += ShowWherePlayerCanMove;
            CityAction.ShowWherePlayerCanMoveFrom += ShowWherePlayerCanMoveFrom;
            CityAction.ShowWhereThePlayerCanRetreat += ShowWhereThePlayerCanRetreat;
            CityAction.ShowWhereZoneToPlayerID += ShowWhereZoneNeutralUnit;
            CityAction.ShowManyZonePlayerInMap += ShowManyZonePlayerInMap;

            CityAction.DeactivateAllTower += DeactivateAllTower;
        }

        private void UpdateCanInteractiveMap()
        {
            var towerEntities = _dataWorld.Select<DistrictComponent>().GetEntities();
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();
            
            var playerComponent = playerEntity.GetComponent<PlayerComponent>();
            
            foreach (var towerEntity in towerEntities)
            {
                var towerComponent = towerEntity.GetComponent<DistrictComponent>();

                if (towerComponent.PlayerControlEntity == PlayerControlEntity.PlayerControl
                    && towerComponent.DistrictBelongPlayerID == playerComponent.PlayerID)
                {
                    towerComponent.DistrictMono.OnInteractiveTower();
                    towerComponent.DistrictMono.OpenInteractiveZoneVisualEffect();
                }
                else
                {
                    towerComponent.DistrictMono.OffInteractiveTower();
                    towerComponent.DistrictMono.CloseInteractiveZoneVisualEffect();
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

            var towerQuery = _dataWorld.Select<DistrictComponent>()
                .Where<DistrictComponent>(tower => tower.PlayerControlEntity == PlayerControlEntity.PlayerControl
                    && tower.DistrictBelongPlayerID == playerComponent.PlayerID);

            var countTowerEntity = towerQuery.Count();
            var towerEntities = towerQuery.GetEntities();

            foreach (var towerEntity in towerEntities)
            {
                var towerComponent = towerEntity.GetComponent<DistrictComponent>();

                if (countTowerEntity > 1)
                {
                    towerComponent.DistrictMono.OnInteractiveTower();
                    towerComponent.DistrictMono.OpenInteractiveZoneVisualEffect();   
                }

                foreach (var towerConnect in towerComponent.DistrictMono.ZoneConnect)
                {
                    towerConnect.OnInteractiveTower();
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
            
            var towerEntity = _dataWorld.Select<DistrictComponent>()
                .Where<DistrictComponent>(tower => tower.GUID == GUIDTower)
                .SelectFirstEntity();

            var towerComponent = towerEntity.GetComponent<DistrictComponent>();

            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();

            var playerComponent = playerEntity.GetComponent<PlayerComponent>();
            foreach (var towerConnect in towerComponent.DistrictMono.ZoneConnect)
            {
                var towerConnectEntity = _dataWorld.Select<DistrictComponent>()
                    .Where<DistrictComponent>(tower => tower.GUID == towerConnect.GUID)
                    .SelectFirstEntity();

                var towerConnectComponent = towerConnectEntity.GetComponent<DistrictComponent>();
                if (towerConnectComponent.PlayerControlEntity == PlayerControlEntity.PlayerControl
                    && towerConnectComponent.DistrictBelongPlayerID == playerComponent.PlayerID)
                {
                    towerConnectComponent.DistrictMono.OpenInteractiveZoneVisualEffect();
                }
            }
        }
        
        /// <summary>
        /// Активирует зоны в которые игрок может отступить
        /// Т.е. задаем стартовый район, и активируем соседние где есть присутствие
        /// </summary>
        private void ShowWhereThePlayerCanRetreat(string GUIDTower, int playerID)
        {
            DeactivateAllTower();
            
            var towerEntity = _dataWorld.Select<DistrictComponent>()
                .Where<DistrictComponent>(tower => tower.GUID == GUIDTower)
                .SelectFirstEntity();

            var towerComponent = towerEntity.GetComponent<DistrictComponent>();
            
            foreach (var towerConnect in towerComponent.DistrictMono.ZoneConnect)
            {
                var towerConnectEntity = _dataWorld.Select<DistrictComponent>()
                    .Where<DistrictComponent>(tower => tower.GUID == towerConnect.GUID)
                    .SelectFirstEntity();

                var towerConnectComponent = towerConnectEntity.GetComponent<DistrictComponent>();
                if (towerConnectComponent.PlayerControlEntity == PlayerControlEntity.PlayerControl
                    && towerConnectComponent.DistrictBelongPlayerID == playerID)
                {
                    towerConnectComponent.DistrictMono.OpenInteractiveZoneVisualEffect();
                    towerConnectComponent.DistrictMono.OnInteractiveTower();
                }
            }
        }

        private void DeactivateAllTower()
        {
            var towerEntities = _dataWorld.Select<DistrictComponent>().GetEntities();
            foreach (var towerEntity in towerEntities)
            {
                var towerComponent = towerEntity.GetComponent<DistrictComponent>();
                towerComponent.DistrictMono.OffInteractiveTower();
                towerComponent.DistrictMono.CloseInteractiveZoneVisualEffect();
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
                    if (towerGuid == unitComponent.GUIDDistrict)
                    {
                        isDouble = true;
                        break;
                    }
                }
                
                if (!isDouble)
                    uniqueTowerGUID.Add(unitComponent.GUIDDistrict);
            }

            foreach (var towerGUID in uniqueTowerGUID)
            {
                var towerEntity = _dataWorld.Select<DistrictComponent>()
                    .Where<DistrictComponent>(tower => tower.GUID == towerGUID)
                    .SelectFirstEntity();
                
                var towerComponent = towerEntity.GetComponent<DistrictComponent>();
                towerComponent.DistrictMono.OnInteractiveTower();
                towerComponent.DistrictMono.OpenInteractiveZoneVisualEffect();
            }
        }

        public void Destroy()
        {
            CityAction.UpdateCanInteractiveMap -= UpdateCanInteractiveMap;
            CityAction.ShowWherePlayerCanMove -= ShowWherePlayerCanMove;
            CityAction.ShowWherePlayerCanMoveFrom -= ShowWherePlayerCanMoveFrom;
            CityAction.ShowWhereThePlayerCanRetreat -= ShowWhereThePlayerCanRetreat;
            CityAction.ShowWhereZoneToPlayerID -= ShowWhereZoneNeutralUnit;
            CityAction.ShowManyZonePlayerInMap -= ShowManyZonePlayerInMap;
            
            CityAction.DeactivateAllTower -= DeactivateAllTower;
        }
    }
}