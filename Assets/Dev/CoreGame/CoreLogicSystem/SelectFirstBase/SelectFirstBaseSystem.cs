using CyberNet.Core.City;
using CyberNet.Core.UI;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace CyberNet.Core.SelectFirstBase
{
    [EcsSystem(typeof(CoreModule))]
    public class SelectFirstBaseSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            SelectFirstBaseAction.CheckInstallFirstBase += CheckPlayerSelectFirstBase;
            SelectFirstBaseAction.SelectBase += SelectBase;
        }

        private bool CheckPlayerSelectFirstBase()
        {
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;

            var isNotInstallFirstBase = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player => player.PlayerID == currentPlayerID)
                .With<PlayerNotInstallFirstBaseComponent>()
                .TrySelectFirstEntity(out var entity);

            if (isNotInstallFirstBase)
                SelectFirstBase();

            return !isNotInstallFirstBase;
        }

        private void SelectFirstBase()
        {
            ref var uiSelectFirstBase = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.SelectFirstBaseUIMono;
            uiSelectFirstBase.OpenWindow();
            CityAction.ShowFirstBaseTower?.Invoke();
        }
        
        private void SelectBase(string towerGUID)
        {
            ref var currentPlayerID = ref _dataWorld.OneData<RoundData>().CurrentPlayerID;
            var towerEntity = _dataWorld.Select<TowerComponent>()
                .Where<TowerComponent>(tower => tower.GUID == towerGUID)
                .SelectFirstEntity();

            ref var towerComponent = ref towerEntity.GetComponent<TowerComponent>();
            
            var entitiesUnit = _dataWorld.Select<UnitComponent>()
                .Where<UnitComponent>(unit => unit.GUIDPoint == towerGUID)
                .GetEntities();

            var closeSolidPoint = -1;
            foreach (var entityUnit in entitiesUnit)
            {
                var unitComponent = entityUnit.GetComponent<UnitComponent>();
                if (unitComponent.IndexPoint > closeSolidPoint)
                    closeSolidPoint = unitComponent.IndexPoint;
            }

            var initUnit = new InitUnitStruct {
                KeyUnit = "blue_unit",
                SolidPoint  = towerComponent.SolidPointMono[closeSolidPoint +1],
                PlayerControl = PlayerControlEnum.Player,
                TargetPlayerID = currentPlayerID
            };

            CityAction.InitUnit?.Invoke(initUnit);

            towerEntity.RemoveComponent<FirstBasePlayerComponent>();
            ref var uiSelectFirstBase = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.SelectFirstBaseUIMono;
            uiSelectFirstBase.CloseWindow();

            CityAction.HideFirstBaseTower?.Invoke();
            RoundAction.StartTurn?.Invoke();
        }
    }
}