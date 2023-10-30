using System.Collections.Generic;
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
                    PlayerIsBelong = PlayerControlEnum.None,
                };
                
                foreach (var squadZone in tower.SquadZonesMono)
                {
                    var countUnit =  InitStartUnitReturnCount(squadZone);
                    if (countUnit > 0)
                    {
                        towerComponent.PlayerIsBelong = PlayerControlEnum.Neutral;
                    }
                }

                entity.AddComponent(towerComponent);
                if (tower.IsFirstBasePlayer)
                    entity.AddComponent(new FirstBasePlayerComponent());
            }
            
            CityAction.UpdatePlayerViewCity?.Invoke();
        }

        private int InitStartUnitReturnCount(SquadZoneMono squadZone)
        {
            var countInit = 0;
            
            if (squadZone.StartIsNeutralSolid)
            {
                var neutralUnit = new InitUnitStruct 
                {
                    KeyUnit = "neutral_unit",
                    SquadZone = squadZone,
                    PlayerControl = PlayerControlEnum.Neutral,
                    TargetPlayerID = -1
                };
                
                InitUnit(neutralUnit);
                countInit++;
            }

            return countInit;
        }

        public void InitUnit(InitUnitStruct unit)
        {
            var cityVisualSO = _dataWorld.OneData<BoardGameData>().CitySO;
            var solidConteiner = _dataWorld.OneData<CityData>().SolidConteiner;
            cityVisualSO.UnitDictionary.TryGetValue(unit.KeyUnit, out var visualUnit);
            
            var unitIcons = Object.Instantiate(visualUnit.IconsUnitMap, solidConteiner.transform);
            
            var allEntitySquadTargetPoint = _dataWorld.Select<SquadMapComponent>()
                .Where<SquadMapComponent>(squadMap => squadMap.GUIDPoint == squadMap.GUIDPoint
                    && squadMap.IndexPoint == unit.SquadZone.Index)
                .GetEntities();

            var positionAllPoint = new List<Vector3>();
            foreach (var entity in allEntitySquadTargetPoint)
            {
                positionAllPoint.Add(entity.GetComponent<SquadMapComponent>().UnitIconsGO.transform.position);
            }
            
            unitIcons.transform.position = CitySupportStatic.SelectPosition(unit.SquadZone.Collider,
                unit.SquadZone.transform.position, positionAllPoint);

            var squadMapComponent = new SquadMapComponent
            {
                GUIDPoint = unit.SquadZone.GUID,
                IndexPoint = unit.SquadZone.Index,
                UnitIconsGO = unitIcons.gameObject,
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
        }

        private void AttackSolidPoint(string guid, int indexPoint)
        {
            var squadEntity = _dataWorld.Select<SquadMapComponent>()
                .Where<SquadMapComponent>(squad => squad.GUIDPoint == guid && squad.IndexPoint == indexPoint)
                .SelectFirstEntity();
            
            ref var squadComponent = ref squadEntity.GetComponent<SquadMapComponent>();
            Object.Destroy(squadComponent.UnitIconsGO);
            squadEntity.Destroy();
        }
    }
}