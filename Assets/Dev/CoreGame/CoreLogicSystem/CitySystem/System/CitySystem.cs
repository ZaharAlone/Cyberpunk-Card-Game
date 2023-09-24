using System.Collections.Generic;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core;
using UnityEngine;

namespace CyberNet.Core.City
{
    [EcsSystem(typeof(CoreModule))]
    public class CitySystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            SetupBoard();
            SetupInteractiveElement();

            CityAction.InitUnit += InitUnit;
            CityAction.AttackSolidPoint += AttackSolidPoint;
        }

        //Создаем поле
        private void SetupBoard()
        {
            var boardGameData = _dataWorld.OneData<BoardGameData>();
            var cityMono = Object.Instantiate(boardGameData.BoardGameConfig.CityMono);
            
            _dataWorld.CreateOneData(new CityData {
                CityGO = cityMono.gameObject,
                CityMono = cityMono,
                SolidConteiner = cityMono.SolidContainer
            });
            GlobalCoreAction.FinishInitGameResource?.Invoke();
        }

        //Инициализируем все интерактивные объекты на карте
        private void SetupInteractiveElement()
        {
            var cityData = _dataWorld.OneData<CityData>();
            var cityVisual = _dataWorld.OneData<BoardGameData>().CitySO;
            
            foreach (var tower in cityData.CityMono.Towers)
            {
                var entity = _dataWorld.NewEntity();
                
                var towerEffect = Object.Instantiate(cityVisual.TowerSelectVFX, tower.transform);
                towerEffect.transform.localScale = tower.GetColliderSize();
                towerEffect.transform.localPosition = new Vector3(0, 0.5f, 0);
                towerEffect.startColor = new Color32(255, 255, 255, 25);
                tower.DeactivateCollider();
                
                var towerComponent = new TowerComponent 
                {
                    GUID = tower.GUID,
                    Key = tower.Key,
                    TowerMono = tower,
                    TowerGO = tower.gameObject,
                    SelectTowerEffect = towerEffect,
                    playerIsBelong = PlayerControlEnum.None,
                    IsFullTowerControl = false,
                    SolidPointMono = tower.SolidPoints
                };

                foreach (var solidPoint in tower.SolidPoints)
                    InitStartUnit(solidPoint);

                entity.AddComponent(towerComponent);
                if (tower.IsFirstBasePlayer)
                    entity.AddComponent(new FirstBasePlayerComponent());
            }
            
            foreach (var connectPoint in cityData.CityMono.ConnectPoints)
            {
                var entity = _dataWorld.NewEntity();

                var connectPointComponent = new ConnectPointComponent() 
                {
                    GUID = connectPoint.GUID,
                    ConnectPointGO = connectPoint.gameObject,
                    ConnectPointMono = connectPoint,
                    SolidPointMono = connectPoint.SolidPointMono,
                    ConnectPointsTypeGUID = CityStaticLogic.SetConnectPointGUIDList(connectPoint)
                };

                InitStartUnit(connectPoint.SolidPointMono);
                entity.AddComponent(connectPointComponent);
            }
        }

        private void InitStartUnit(SolidPointMono solidPoint)
        {
            ref var cityVisual = ref _dataWorld.OneData<BoardGameData>().CitySO;
            
            if (solidPoint.StartIsNeutralSolid)
            {
                var neutralUnit = new InitUnitStruct 
                {
                    KeyUnit = "neutral_unit",
                    SolidPoint = solidPoint,
                    PlayerControl = PlayerControlEnum.Neutral,
                    TargetPlayerID = -1
                };
                
                InitUnit(neutralUnit);
            }
            else
            {
                solidPoint.PointVFX = Object.Instantiate(cityVisual.ClearSolidPointVFX, solidPoint.transform);
            }
        }

        public void InitUnit(InitUnitStruct unit)
        {
            var cityVisualSO = _dataWorld.OneData<BoardGameData>().CitySO;
            var solidConteiner = _dataWorld.OneData<CityData>().SolidConteiner;
            cityVisualSO.UnitDictionary.TryGetValue(unit.KeyUnit, out var visualUnit);

            if (unit.SolidPoint.transform.childCount > 0) 
                Object.Destroy(unit.SolidPoint.transform.GetChild(0).gameObject);
            
            var solidPointVFX = Object.Instantiate(cityVisualSO.SolidPointVFXMono, unit.SolidPoint.transform);
            solidPointVFX.SetColor(visualUnit.ColorUnit);
            unit.SolidPoint.PointVFX = solidPointVFX.gameObject;
            
            var unitMono = Object.Instantiate(visualUnit.UnitMono, solidConteiner.transform);
            unitMono.transform.position = unit.SolidPoint.transform.position;
            
            var unitComponent = new UnitComponent
            {
                GUIDPoint = unit.SolidPoint.GUID,
                IndexPoint = unit.SolidPoint.Index,
                UnitGO = unitMono.gameObject,
                UnitMono = unitMono,
                PlayerControl = unit.PlayerControl,
                PowerSolidPlayerID = unit.TargetPlayerID
            };

            _dataWorld.NewEntity().AddComponent(unitComponent);
        }

        public void Destroy()
        {
            ref var resourceTable = ref _dataWorld.OneData<CityData>();
            Object.Destroy(resourceTable.CityGO);

            _dataWorld.RemoveOneData<CityData>();
        }

        private void AttackSolidPoint(string guid, int indexPoint)
        {
            var unitEntity = _dataWorld.Select<UnitComponent>()
                .Where<UnitComponent>(unit => unit.GUIDPoint == guid && unit.IndexPoint == indexPoint)
                .SelectFirstEntity();
            
            ref var unitComponent = ref unitEntity.GetComponent<UnitComponent>();
            Object.Destroy(unitComponent.UnitGO);
            unitEntity.Destroy();

            ClearSolidPoint(guid, indexPoint);
        }
        
        private void ClearSolidPoint(string guid, int indexPoint)
        {
            var isTowerEntity = _dataWorld.Select<TowerComponent>()
                .Where<TowerComponent>(tower => tower.GUID == guid)
                .TrySelectFirstEntity(out var towerEntity);

            ref var cityVisual = ref _dataWorld.OneData<BoardGameData>().CitySO;

            if (isTowerEntity)
            {
                ref var towerComponent = ref towerEntity.GetComponent<TowerComponent>();
                Object.Destroy(towerComponent.TowerMono.SolidPoints[indexPoint].PointVFX);
                towerComponent.TowerMono.SolidPoints[indexPoint].PointVFX = Object.Instantiate(cityVisual.ClearSolidPointVFX, towerComponent.TowerMono.SolidPoints[indexPoint].transform);
            }
            else
            {
                var connectPointEntity = _dataWorld.Select<ConnectPointComponent>()
                    .Where<ConnectPointComponent>(point => point.GUID == guid)
                    .SelectFirstEntity();

                ref var connectPointComponent = ref connectPointEntity.GetComponent<ConnectPointComponent>();
                Object.Destroy(connectPointComponent.ConnectPointMono.SolidPointMono.PointVFX);
                connectPointComponent.ConnectPointMono.SolidPointMono.PointVFX = Object.Instantiate(cityVisual.ClearSolidPointVFX, connectPointComponent.ConnectPointMono.SolidPointMono.transform);
            }
        }
    }
}