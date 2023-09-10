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
            var cityVisual = _dataWorld.OneData<BoardGameData>().CityVisualSO;
            
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
                    entity.AddComponent(new FirstBasePlayer());
            }
            
            foreach (var connectPoint in cityData.CityMono.ConnectPoints)
            {
                var entity = _dataWorld.NewEntity();

                var connectPointComponent = new ConnectPointComponent() 
                {
                    GUID = connectPoint.GUID,
                    ConnectPointGO = connectPoint.gameObject,
                    SolidPointMono = connectPoint.SolidPointMono
                };

                InitStartUnit(connectPoint.SolidPointMono);
                entity.AddComponent(connectPointComponent);
            }
        }

        private void InitStartUnit(SolidPointMono solidPoint)
        {
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
                _dataWorld.OneData<BoardGameData>().CityVisualSO.UnitCityVFX.TryGetValue("clear_solid_point", out var clearPoint_vfx);
                Object.Instantiate(clearPoint_vfx, solidPoint.transform);
            }
        }

        public void InitUnit(InitUnitStruct unit)
        {
            var cityVisualSO = _dataWorld.OneData<BoardGameData>().CityVisualSO;
            var solidConteiner = _dataWorld.OneData<CityData>().SolidConteiner;
            cityVisualSO.UnitDictionary.TryGetValue(unit.KeyUnit, out var targetUnit);
            cityVisualSO.UnitCityVFX.TryGetValue(unit.KeyUnit, out var unitPoint_vfx);

            if (unit.SolidPoint.transform.childCount > 0) 
                Object.Destroy(unit.SolidPoint.transform.GetChild(0).gameObject);
            Object.Instantiate(unitPoint_vfx, unit.SolidPoint.transform);
            
            var unitMono = Object.Instantiate(targetUnit, solidConteiner.transform);
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
    }
}