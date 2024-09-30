using System.Collections.Generic;
using CyberNet.Global.Sound;
using CyberNet.Tools;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace CyberNet.Core.Map
{
    /// <summary>
    /// Система управляет юнитами их созданием, уничтожением
    /// </summary>
    [EcsSystem(typeof(CoreModule))]
    public class CityUnitSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            CityAction.InitUnit += InitUnit;
            CityAction.AttackSolidPoint += AttackSolidPoint;
            CityAction.ActivationsColliderUnitsInTower += ActivationsColliderUnitsInTower;
            CityAction.DeactivationsColliderAllUnits += DeactivationsColliderAllUnits;
        }

        public void InitUnit(InitUnitStruct unit)
        {
            var cityVisualSO = _dataWorld.OneData<BoardGameData>().CitySO;
            var solidConteiner = _dataWorld.OneData<CityData>().SolidConteiner;
            cityVisualSO.UnitDictionary.TryGetValue(unit.KeyUnit, out var visualUnit);
            
            var unitIconsMono = Object.Instantiate(cityVisualSO.IconsContainerUnitMap, solidConteiner.transform);
            unitIconsMono.SetViewUnit(visualUnit.IconsUnit, visualUnit.ColorUnit);
            unitIconsMono.DeactivateCollider();
            
            var allEntitySquadTargetPoint = _dataWorld.Select<UnitMapComponent>()
                .Where<UnitMapComponent>(squadMap => squadMap.GUIDDistrict == squadMap.GUIDDistrict
                    && squadMap.IndexPoint == unit.UnitZone.Index)
                .GetEntities();

            var positionAllPoint = new List<Vector3>();
            foreach (var entity in allEntitySquadTargetPoint)
            {
                positionAllPoint.Add(entity.GetComponent<UnitMapComponent>().UnitIconsGO.transform.position);
            }
            
            unitIconsMono.transform.position = CitySupportStatic.SelectPosition(unit.UnitZone.Collider,
                unit.UnitZone.transform.position, positionAllPoint);
            
            var guidUnit = CreateGUID.Create();
            unitIconsMono.SetGUID(guidUnit);
            
            var squadMapComponent = new UnitMapComponent
            {
                GUIDDistrict = unit.UnitZone.GUIDTower,
                IndexPoint = unit.UnitZone.Index,
                GUIDUnit = guidUnit,
                UnitIconsGO = unitIconsMono.gameObject,
                IconsUnitInMapMono = unitIconsMono,
                PlayerControl = unit.PlayerControl,
                PowerSolidPlayerID = unit.TargetPlayerID
            };
            
            _dataWorld.NewEntity().AddComponent(squadMapComponent);
            
            
            if (_dataWorld.OneData<RoundData>().PauseInteractive)
                SoundAction.PlaySound?.Invoke(_dataWorld.OneData<SoundData>().Sound.AddUnitInMap);
        }

        private void AttackSolidPoint(string guid, int indexPoint)
        {
            var squadEntity = _dataWorld.Select<UnitMapComponent>()
                .Where<UnitMapComponent>(squad => squad.GUIDDistrict == guid && squad.IndexPoint == indexPoint)
                .SelectFirstEntity();
            
            ref var squadComponent = ref squadEntity.GetComponent<UnitMapComponent>();
            Object.Destroy(squadComponent.UnitIconsGO);
            squadEntity.Destroy();
        }

        private void ActivationsColliderUnitsInTower(string guidTower, int playerID)
        {
            var selectTowersForActivateUnits = new List<string>();
            
            var targetTower = _dataWorld.Select<DistrictComponent>()
                .Where<DistrictComponent>(tower => tower.GUID == guidTower)
                .SelectFirstEntity();
            var towerComponent = targetTower.GetComponent<DistrictComponent>();
            
            foreach (var towerConnect in towerComponent.DistrictMono.ZoneConnect)
            {
                var towerConnectEntity = _dataWorld.Select<DistrictComponent>()
                    .Where<DistrictComponent>(tower => tower.GUID == towerConnect.GUID)
                    .SelectFirstEntity();

                var towerConnectComponent = towerConnectEntity.GetComponent<DistrictComponent>();
                if (towerConnectComponent.PlayerControlEntity == PlayerControlEntity.PlayerControl
                    && towerConnectComponent.DistrictBelongPlayerID == playerID)
                {
                    selectTowersForActivateUnits.Add(towerConnectComponent.GUID);
                }
            }

            foreach (var selectTowerGUID in selectTowersForActivateUnits)
            {
                var squadEntities = _dataWorld.Select<UnitMapComponent>()
                    .Where<UnitMapComponent>(unit => unit.GUIDDistrict == selectTowerGUID && unit.PowerSolidPlayerID == playerID)
                    .GetEntities();

                foreach (var squadEntity in squadEntities)
                {
                    var squadComponent = squadEntity.GetComponent<UnitMapComponent>();
                    squadComponent.IconsUnitInMapMono.ActivateCollider();
                }   
            }
        }

        private void DeactivationsColliderAllUnits()
        {
            var squadEntities = _dataWorld.Select<UnitMapComponent>().GetEntities();

            foreach (var squadEntity in squadEntities)
            {
                var squadComponent = squadEntity.GetComponent<UnitMapComponent>();
                squadComponent.IconsUnitInMapMono.DeactivateCollider();
            }
        }
        
        public void Destroy()
        {
            CityAction.InitUnit -= InitUnit;
            CityAction.AttackSolidPoint -= AttackSolidPoint;
            CityAction.ActivationsColliderUnitsInTower -= ActivationsColliderUnitsInTower;
            CityAction.DeactivationsColliderAllUnits -= DeactivationsColliderAllUnits;

            var unitEntities = _dataWorld.Select<UnitMapComponent>().GetEntities();

            foreach (var entity in unitEntities)
            {
                entity.Destroy();
            }
        }
    }
}