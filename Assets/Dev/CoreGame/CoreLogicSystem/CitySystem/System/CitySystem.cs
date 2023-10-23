using System.Collections.Generic;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using Unity.VisualScripting;
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
            //TODO Пересмотреть
            var cityData = _dataWorld.OneData<CityData>();
            var cityVisual = _dataWorld.OneData<BoardGameData>().CitySO;
            
            foreach (var tower in cityData.CityMono.Towers)
            {
                var entity = _dataWorld.NewEntity();
                
                /*
                var towerEffect = Object.Instantiate(cityVisual.TowerSelectVFX, tower.transform);
                towerEffect.transform.localScale = tower.GetColliderSize();
                towerEffect.transform.localPosition = new Vector3(0, 0.5f, 0);
                towerEffect.startColor = new Color32(255, 255, 255, 25);
                */
                tower.DeactivateCollider();
                
                var towerComponent = new TowerComponent 
                {
                    GUID = tower.GUID,
                    Key = tower.Key,
                    TowerMono = tower,
                    TowerGO = tower.gameObject,
                    SquadZonesMono = tower.SquadZonesMono,
                    //SelectTowerEffect = towerEffect,
                    playerIsBelong = PlayerControlEnum.None,
                };

                foreach (var squadZone in tower.SquadZonesMono)
                {
                    InitStartUnit(squadZone);
                }

                entity.AddComponent(towerComponent);
                if (tower.IsFirstBasePlayer)
                    entity.AddComponent(new FirstBasePlayerComponent());
            }
        }

        private void InitStartUnit(SquadZoneMono squadZone)
        {
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
            }
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
            unitIcons.transform.position = SelectPosition(unit.SquadZone.Collider, unit.SquadZone.transform.position, positionAllPoint);
            //unit.SquadZone.transform.position;

            
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
        
        private Vector3 SelectPosition(BoxCollider collider, Vector3 positions, List<Vector3> positionsOtherItem)
        {
            var y = positions.y;
            var x = collider.size.x / 2;
            var z = collider.size.z / 2;
            var newPos = new Vector3();

            var noDouble = false;
            while (!noDouble)
            {
                newPos = new Vector3(Random.Range(-x, x), y, Random.Range(-z, z));
                newPos.x += collider.center.x + positions.x;
                newPos.z += collider.center.z + positions.z;

                noDouble = CheckDistanceObject(newPos, positionsOtherItem);
            }
            return newPos;
        }

        //check the distance between other objects so as not to plant plants too close
        private bool CheckDistanceObject(Vector3 positions, List<Vector3> positionsOtherItem)
        {
            if (positionsOtherItem.Count == 0)
                return true;

            var result = true;
            foreach (var item in positionsOtherItem)
                if (Vector3.Distance(item, positions) < 1)
                    result = false;
            return result;
        }
    }
}