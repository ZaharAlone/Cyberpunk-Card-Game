using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
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
                    squadPointMono = connectPoint.squadPointMono,
                    ConnectPointsTypeGUID = CityStaticLogic.SetConnectPointGUIDList(connectPoint)
                };

                InitStartUnit(connectPoint.squadPointMono);
                entity.AddComponent(connectPointComponent);
            }
        }

        private void InitStartUnit(SquadPointMono squadPoint)
        {
            ref var cityVisual = ref _dataWorld.OneData<BoardGameData>().CitySO;
            
            if (squadPoint.StartIsNeutralSolid)
            {
                var neutralUnit = new InitUnitStruct 
                {
                    KeyUnit = "neutral_unit",
                    squadPoint = squadPoint,
                    PlayerControl = PlayerControlEnum.Neutral,
                    TargetPlayerID = -1
                };
                
                InitUnit(neutralUnit);
            }
            else
            {
                squadPoint.PointVFX = Object.Instantiate(cityVisual.ClearSolidPointVFX, squadPoint.transform);
            }
        }

        public void InitUnit(InitUnitStruct unit)
        {
            var cityVisualSO = _dataWorld.OneData<BoardGameData>().CitySO;
            var solidConteiner = _dataWorld.OneData<CityData>().SolidConteiner;
            cityVisualSO.UnitDictionary.TryGetValue(unit.KeyUnit, out var visualUnit);

            if (unit.squadPoint.transform.childCount > 0) 
                Object.Destroy(unit.squadPoint.transform.GetChild(0).gameObject);
            
            var solidPointVFX = Object.Instantiate(cityVisualSO.squadPointVFXMono, unit.squadPoint.transform);
            solidPointVFX.SetColor(visualUnit.ColorUnit);
            unit.squadPoint.PointVFX = solidPointVFX.gameObject;
            
            var unitMono = Object.Instantiate(visualUnit.squadMono, solidConteiner.transform);
            unitMono.transform.position = unit.squadPoint.transform.position;
            
            var unitComponent = new SquadComponent
            {
                GUIDPoint = unit.squadPoint.GUID,
                IndexPoint = unit.squadPoint.Index,
                SquadGO = unitMono.gameObject,
                SquadMono = unitMono,
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
            var squadEntity = _dataWorld.Select<SquadComponent>()
                .Where<SquadComponent>(squad => squad.GUIDPoint == guid && squad.IndexPoint == indexPoint)
                .SelectFirstEntity();
            
            ref var squadComponent = ref squadEntity.GetComponent<SquadComponent>();
            Object.Destroy(squadComponent.SquadGO);
            squadEntity.Destroy();

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
                Object.Destroy(connectPointComponent.ConnectPointMono.squadPointMono.PointVFX);
                connectPointComponent.ConnectPointMono.squadPointMono.PointVFX = Object.Instantiate(cityVisual.ClearSolidPointVFX, connectPointComponent.ConnectPointMono.squadPointMono.transform);
            }
        }
    }
}