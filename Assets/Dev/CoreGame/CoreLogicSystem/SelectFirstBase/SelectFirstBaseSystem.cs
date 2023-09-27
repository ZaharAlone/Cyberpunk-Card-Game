using CyberNet.Core.City;
using CyberNet.Core.Player;
using CyberNet.Core.UI;
using CyberNet.Global;
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
                .TrySelectFirstEntity(out var playerEntity);

            if (!isNotInstallFirstBase)
                return true;
            
            ref var playerComponent = ref playerEntity.GetComponent<PlayerComponent>();

            if (isNotInstallFirstBase && playerComponent.playerTypeEnum == PlayerTypeEnum.Player)
            {
                SelectFirstBase();
            }

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
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player => player.PlayerID == currentPlayerID)
                .SelectFirstEntity();
            
            ref var playerComponent = ref playerEntity.GetComponent<PlayerComponent>();
            ref var playerVisualComponent = ref playerEntity.GetComponent<PlayerViewComponent>();
            
            var towerEntity = _dataWorld.Select<TowerComponent>()
                .Where<TowerComponent>(tower => tower.GUID == towerGUID)
                .SelectFirstEntity();

            ref var towerComponent = ref towerEntity.GetComponent<TowerComponent>();
            
            var entitiesUnit = _dataWorld.Select<SquadComponent>()
                .Where<SquadComponent>(unit => unit.GUIDPoint == towerGUID)
                .GetEntities();

            var closeSolidPoint = -1;
            foreach (var entityUnit in entitiesUnit)
            {
                var unitComponent = entityUnit.GetComponent<SquadComponent>();
                if (unitComponent.IndexPoint > closeSolidPoint)
                    closeSolidPoint = unitComponent.IndexPoint;
            }

            var initUnit = new InitUnitStruct {
                KeyUnit = playerVisualComponent.KeyCityVisual,
                squadPoint  = towerComponent.SolidPointMono[closeSolidPoint +1],
                PlayerControl = PlayerControlEnum.Player,
                TargetPlayerID = currentPlayerID
            };

            CityAction.InitUnit?.Invoke(initUnit);

            playerComponent.UnitCount--;
            towerEntity.RemoveComponent<FirstBasePlayerComponent>();
            playerEntity.RemoveComponent<PlayerNotInstallFirstBaseComponent>();
            
            ref var uiSelectFirstBase = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.SelectFirstBaseUIMono;
            uiSelectFirstBase.CloseWindow();

            CityAction.HideFirstBaseTower?.Invoke();
            RoundAction.StartTurn?.Invoke();
            BoardGameUIAction.UpdateStatsPlayersPassportUI?.Invoke();
        }
    }
}