using System.Collections.Generic;
using CyberNet.Tools;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace CyberNet.Core.City
{
    [EcsSystem(typeof(CoreModule))]
    public class CitySystem : IPreInitSystem, IInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            CityAction.InitUnit += InitUnit;
            CityAction.AttackSolidPoint += AttackSolidPoint;
        }

        public void Init()
        {
            SetupBoard();
            SetupInteractiveElement();
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
            //TODO Пересмотреть Добавить визуал к кускам карты
            var cityData = _dataWorld.OneData<CityData>();
            
            foreach (var tower in cityData.CityMono.Towers)
            {
                var entity = _dataWorld.NewEntity();
                tower.DeactivateCollider();
                tower.CloseInteractiveZoneVisualEffect();
                
                var towerComponent = new TowerComponent 
                {
                    GUID = tower.GUID,
                    Key = tower.Key,
                    TowerMono = tower,
                    TowerGO = tower.gameObject,
                    SquadZonesMono = tower.SquadZonesMono,
                    VisualEffectZone = tower.VisualEffectZone,
                    PlayerControlEntity = PlayerControlEntity.None,
                };
                
                foreach (var squadZone in tower.SquadZonesMono)
                {
                    var countUnit =  InitStartUnitReturnCount(squadZone);
                    if (countUnit > 0)
                    {
                        towerComponent.PlayerControlEntity = PlayerControlEntity.Neutral;
                    }
                }

                entity.AddComponent(towerComponent);
                if (tower.IsFirstBasePlayer)
                    entity.AddComponent(new FirstBasePlayerComponent());
            }
            
            CityAction.UpdatePlayerViewCity?.Invoke();
        }

        private int InitStartUnitReturnCount(UnitZoneMono UnitZone)
        {
            var countInit = 0;
            var countNeutralUnitInTower = _dataWorld.OneData<BoardGameData>().BoardGameRule.CountNeutralUnitInTower;
            if (UnitZone.StartIsNeutralSolid)
            {
                for (int i = 0; i < countNeutralUnitInTower; i++)
                {
                    var neutralUnit = new InitUnitStruct 
                    {
                        KeyUnit = "neutral_unit",
                        UnitZone = UnitZone,
                        PlayerControl = PlayerControlEntity.Neutral,
                        TargetPlayerID = -1
                    };
                
                    InitUnit(neutralUnit);
                    countInit++;   
                }
            }

            return countInit;
        }

        public void InitUnit(InitUnitStruct unit)
        {
            var cityVisualSO = _dataWorld.OneData<BoardGameData>().CitySO;
            var solidConteiner = _dataWorld.OneData<CityData>().SolidConteiner;
            cityVisualSO.UnitDictionary.TryGetValue(unit.KeyUnit, out var visualUnit);
            
            var unitIconsMono = Object.Instantiate(visualUnit.IconsUnitMap, solidConteiner.transform);
            
            var allEntitySquadTargetPoint = _dataWorld.Select<UnitMapComponent>()
                .Where<UnitMapComponent>(squadMap => squadMap.GUIDTower == squadMap.GUIDTower
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
                GUIDTower = unit.UnitZone.GUIDTower,
                IndexPoint = unit.UnitZone.Index,
                GUIDUnit = guidUnit,
                UnitIconsGO = unitIconsMono.gameObject,
                IconsUnitInMapMono = unitIconsMono,
                PlayerControl = unit.PlayerControl,
                PowerSolidPlayerID = unit.TargetPlayerID
            };

            _dataWorld.NewEntity().AddComponent(squadMapComponent);
        }

        public void Destroy()
        {
            ref var resourceTable = ref _dataWorld.OneData<CityData>();
            Object.Destroy(resourceTable.CityGO);

            _dataWorld.RemoveOneData<CityData>();
            
            CityAction.InitUnit -= InitUnit;
            CityAction.AttackSolidPoint -= AttackSolidPoint;
        }

        private void AttackSolidPoint(string guid, int indexPoint)
        {
            var squadEntity = _dataWorld.Select<UnitMapComponent>()
                .Where<UnitMapComponent>(squad => squad.GUIDTower == guid && squad.IndexPoint == indexPoint)
                .SelectFirstEntity();
            
            ref var squadComponent = ref squadEntity.GetComponent<UnitMapComponent>();
            Object.Destroy(squadComponent.UnitIconsGO);
            squadEntity.Destroy();
        }
    }
}