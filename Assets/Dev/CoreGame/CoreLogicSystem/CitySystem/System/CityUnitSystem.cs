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
            CityAction.ActivationsColliderUnitsInDistrict += ActivationsColliderUnitsInDistrict;
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

        private void ActivationsColliderUnitsInDistrict(string guidDistrict, int playerID)
        {
            var targetDistrict = _dataWorld.Select<DistrictComponent>()
                .Where<DistrictComponent>(district => district.GUID == guidDistrict)
                .SelectFirstEntity();
            var districtComponent = targetDistrict.GetComponent<DistrictComponent>();
         
            var selectDistrictForActivateUnits = new List<string>();
            
            foreach (var districtConnect in districtComponent.DistrictMono.ZoneConnect)
            {
                var towerConnectEntity = _dataWorld.Select<DistrictComponent>()
                    .Where<DistrictComponent>(tower => tower.GUID == districtConnect.GUID)
                    .SelectFirstEntity();

                var districtConnectComponent = towerConnectEntity.GetComponent<DistrictComponent>();
                var isDistrictPlayerControl = districtConnectComponent.PlayerControlEntity == PlayerControlEntity.PlayerControl;
                var districtBelongTargetPlayer = districtConnectComponent.DistrictBelongPlayerID == playerID;
                
                if (isDistrictPlayerControl && districtBelongTargetPlayer)
                    selectDistrictForActivateUnits.Add(districtConnectComponent.GUID);
            }

            foreach (var selectDistrictGUID in selectDistrictForActivateUnits)
            {
                var squadEntities = _dataWorld.Select<UnitMapComponent>()
                    .Where<UnitMapComponent>(unit => unit.GUIDDistrict == selectDistrictGUID && unit.PowerSolidPlayerID == playerID)
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
            CityAction.ActivationsColliderUnitsInDistrict -= ActivationsColliderUnitsInDistrict;
            CityAction.DeactivationsColliderAllUnits -= DeactivationsColliderAllUnits;

            var unitEntities = _dataWorld.Select<UnitMapComponent>().GetEntities();

            foreach (var entity in unitEntities)
            {
                entity.Destroy();
            }
        }
    }
}