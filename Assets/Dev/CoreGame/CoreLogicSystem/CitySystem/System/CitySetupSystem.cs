using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.Map;
using Object = UnityEngine.Object;

namespace CyberNet.Core
{
    /// <summary>
    /// Система первично настраивает город, создает карту и расставляет нейтральных юнитов
    /// </summary>
    [EcsSystem(typeof(CoreModule))]
    public class CitySetupSystem : IInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

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
            var districtConfig = _dataWorld.OneData<BoardGameData>().DistrictConfig;
            
            foreach (var district in cityData.CityMono.Disctrict)
            {
                var entity = _dataWorld.NewEntity();
                district.OffInteractiveTower();
                district.CloseInteractiveZoneVisualEffect();
                
                var towerComponent = new DistrictComponent 
                {
                    GUID = district.GUID,
                    Key = district.Key,
                    DistrictMono = district,
                    TowerGO = district.gameObject,
                    SquadZonesMono = district.SquadZonesMono,
                    VisualEffectZone = district.VisualEffectZone,
                    PlayerControlEntity = PlayerControlEntity.None,
                    BonusDistrict = districtConfig[district.Key].Bonus,
                };
                
                foreach (var squadZone in district.SquadZonesMono)
                {
                    var countUnit =  InitStartUnitReturnCount(squadZone);
                    if (countUnit > 0)
                    {
                        towerComponent.PlayerControlEntity = PlayerControlEntity.NeutralUnits;
                    }
                }

                entity.AddComponent(towerComponent);
                if (district.IsFirstBasePlayer)
                    entity.AddComponent(new FirstBasePlayerComponent());
            }
            
            CityAction.UpdateTowerControlView?.Invoke();
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
                        PlayerControl = PlayerControlEntity.NeutralUnits,
                        TargetPlayerID = -1
                    };
                
                    CityAction.InitUnit?.Invoke(neutralUnit);
                    countInit++;   
                }
            }

            return countInit;
        }

        public void Destroy()
        {
            ref var resourceTable = ref _dataWorld.OneData<CityData>();
            Object.Destroy(resourceTable.CityGO);

            _dataWorld.RemoveOneData<CityData>();

            var towerEntities = _dataWorld.Select<DistrictComponent>().GetEntities();
            foreach (var tower in towerEntities)
            {
                tower.Destroy();
            }
        }
    }
}