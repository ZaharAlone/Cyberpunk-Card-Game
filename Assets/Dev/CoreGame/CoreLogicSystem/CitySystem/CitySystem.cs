using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core;
using Object = UnityEngine.Object;

namespace CyberNet.CoreGame.City
{
    [EcsSystem(typeof(CoreModule))]
    public class CitySystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
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
                SolidConteiner = cityMono.SolidConteiner
            });
        }
        
        //Инициализируем все интерактивные объекты на карте
        private void SetupInteractiveElement()
        {
            var cityData = _dataWorld.OneData<CityData>();

            foreach (var tower in cityData.CityMono.Towers)
            {
                var entity = _dataWorld.NewEntity();

                var towerComponent = new TowerComponent 
                {
                    GUID = tower.GUID,
                    TowerGO = tower.gameObject,
                    PowerTowerBelong = PlayerEnum.Neutral,
                    IsFullPower = true,
                    SolidPointMono = tower.SolidPoints
                };

                foreach (var solidPoint in tower.SolidPoints)
                    InitNeutralUnit(solidPoint);

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

                InitNeutralUnit(connectPoint.SolidPointMono);
                entity.AddComponent(connectPointComponent);
            }
        }

        private void InitNeutralUnit(SolidPointMono solidPoint)
        {
            if (solidPoint.StartIsNeutralSolid)
            {
                InitUnit("neutral_unit", solidPoint, PlayerEnum.Neutral);   
            }
            else
            {
                _dataWorld.OneData<BoardGameData>().CityVisualSO.UnitCityVFX.TryGetValue("clear_solid_point", out var clearPoint_vfx);
                Object.Instantiate(clearPoint_vfx, solidPoint.transform);
            }
        }

        private void InitUnit(string keyUnit, SolidPointMono solidPoint, PlayerEnum targetPlayer)
        {
            var cityVisualSO = _dataWorld.OneData<BoardGameData>().CityVisualSO;
            var solidConteiner = _dataWorld.OneData<CityData>().SolidConteiner;
            cityVisualSO.UnitDictionary.TryGetValue(keyUnit, out var targetUnit);
            cityVisualSO.UnitCityVFX.TryGetValue(keyUnit, out var unitPoint_vfx);

            if (solidPoint.transform.childCount > 0) 
                Object.Destroy(solidPoint.transform.GetChild(0).gameObject);
            Object.Instantiate(unitPoint_vfx, solidPoint.transform);
            
            var unitMono = Object.Instantiate(targetUnit, solidConteiner.transform);
            unitMono.transform.position = solidPoint.transform.position;
            var unitComponent = new UnitComponent
            {
                GUIDPoint = solidPoint.GUID,
                IndexPoint = solidPoint.Index,
                UnitGO = unitMono.gameObject,
                UnitMono = unitMono,
                PowerSolid = targetPlayer
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