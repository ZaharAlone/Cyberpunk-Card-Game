using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.AI;
using CyberNet.Core.BezierCurveNavigation;
using CyberNet.Core.City;
using CyberNet.Core.Player;
using CyberNet.Core.UI;
using CyberNet.Global;

namespace CyberNet.Core.AbilityCard
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityAddUnitInMapSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

         public void PreInit()
        {
            AbilityCardAction.AddUnitMap += AddUnitMap;
        }
        
        private void AddUnitMap()
        {
            Debug.LogError("Add unit map ability");
            ref var roundData = ref _dataWorld.OneData<RoundData>();

            if (roundData.PlayerTypeEnum != PlayerTypeEnum.Player)
            {
                AbilityAIAction.AddUnitMap?.Invoke();
                return;
            }
            
            //roundData.PauseInteractive = true;
            //AbilitySelectElementAction.OpenSelectAbilityCard?.Invoke(AbilityType.Attack, 0, false);
            BezierCurveNavigationAction.StartBezierCurve?.Invoke();
            //CityAction.SelectTower += SelectTower;
        }
        
        private void AddUnitTower(TowerMono towerMono)
        {
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();

            ref var playerComponent = ref playerEntity.GetComponent<PlayerComponent>();
            ref var playerVisualComponent = ref playerEntity.GetComponent<PlayerViewComponent>();
            playerComponent.UnitCount--;
            var playerID = playerComponent.PlayerID;
            
            var towerEntity = _dataWorld.Select<TowerComponent>()
                .Where<TowerComponent>(tower => tower.GUID == towerMono.GUID)
                .SelectFirstEntity();
            
            ref var towerComponent = ref towerEntity.GetComponent<TowerComponent>();

            int targetSquadZone = 0;
            foreach (SquadZoneMono squadZone in towerComponent.SquadZonesMono)
            {
                bool isClose = _dataWorld.Select<SquadMapComponent>()
                    .Where<SquadMapComponent>(unit => unit.GUIDPoint == towerMono.GUID
                        && unit.IndexPoint == squadZone.Index
                        && unit.PowerSolidPlayerID != playerID)
                    .TrySelectFirstEntity(out Entity t);

                if (isClose)
                    targetSquadZone = squadZone.Index + 1;
                else
                {
                    targetSquadZone = squadZone.Index;
                    break;
                }
            }

            var initUnit = new InitUnitStruct {
                KeyUnit = playerVisualComponent.KeyCityVisual, SquadZone = towerMono.SquadZonesMono[targetSquadZone], PlayerControl = PlayerControlEnum.Player, TargetPlayerID = playerID,
            };

            CityAction.InitUnit?.Invoke(initUnit);
            CityAction.UpdatePresencePlayerInCity?.Invoke();
            BoardGameUIAction.UpdateStatsMainPlayersPassportUI?.Invoke();
            BoardGameUIAction.UpdateStatsPlayersCurrency?.Invoke();
            CityAction.UpdateCanInteractiveMap?.Invoke();
        }
    }
}