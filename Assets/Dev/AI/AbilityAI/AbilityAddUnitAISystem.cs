using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using System.Collections.Generic;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.Map;
using CyberNet.Core.Player;
using CyberNet.Core.UI;

namespace CyberNet.Core.AI.Ability
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityAddUnitAISystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AbilityAIAction.AddUnitMap += AddUnit;
        }

        private void AddUnit(string guidCard)
        {
            //Берем нужный ентити карты, ищем оптимальное место размещения юнита
            //размещаем там юнита. Если нужно разместить несколько, повторяем несколько раз.
            var entityCard = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == guidCard)
                .SelectFirstEntity();
            
            var abilitySelectElementComponent = entityCard.GetComponent<AbilitySelectElementComponent>();

            for (int i = 0; i < abilitySelectElementComponent.AbilityCard.Count; i++)
            {
                FindOptimalTower();
            }
            
            UpdateViewPlayer();
            entityCard.RemoveComponent<AbilitySelectElementComponent>();
        }

        private void FindOptimalTower()
        {
            var currentPlayerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();

            var currentPlayerComponent = currentPlayerEntity.GetComponent<PlayerComponent>();
            var towerEntities = _dataWorld.Select<TowerComponent>()
                .Where<TowerComponent>(tower => tower.TowerBelongPlayerID == currentPlayerComponent.PlayerID
                    && tower.PlayerControlEntity == PlayerControlEntity.PlayerControl)
                .GetEntities();
            
            //Сначала ищем есть ли зоны которые соприкасаются с зонами другого игрока
            //Если есть, ищем ту где наименьшее кол-во юнитов и добавляем туда.
            //Если нет, то ищем наружние зоны с наименьшим кол-вом юнитов.
            
            var guidSelectPotentiallyWeakTower = new List<string>();
            foreach (var towerEntity in towerEntities)
            {
                var towerComponent = towerEntity.GetComponent<TowerComponent>();
                
                foreach (var zoneConnectTower in towerComponent.TowerMono.ZoneConnect)
                {
                    var connectTowerEntity = _dataWorld.Select<TowerComponent>()
                        .Where<TowerComponent>(tower => tower.GUID == zoneConnectTower.GUID)
                        .SelectFirstEntity();
                    var connectTowerComponent = connectTowerEntity.GetComponent<TowerComponent>();

                    if (connectTowerComponent.TowerBelongPlayerID != currentPlayerComponent.PlayerID
                        && connectTowerComponent.PlayerControlEntity == PlayerControlEntity.PlayerControl)
                    {
                        guidSelectPotentiallyWeakTower.Add(towerComponent.GUID);
                        break;
                    }
                }
            }
            
            if (guidSelectPotentiallyWeakTower.Count == 0)
            {
                guidSelectPotentiallyWeakTower.Clear();
                var maxConnectZone = 0;
                foreach (var towerEntity in towerEntities)
                {
                    var towerComponent = towerEntity.GetComponent<TowerComponent>();

                    var countConnectZone = 0;
                    foreach (var zoneConnectTower in towerComponent.TowerMono.ZoneConnect)
                    {
                        var connectTowerEntity = _dataWorld.Select<TowerComponent>()
                            .Where<TowerComponent>(tower => tower.GUID == zoneConnectTower.GUID)
                            .SelectFirstEntity();
                        var connectTowerComponent = connectTowerEntity.GetComponent<TowerComponent>();
        
                        if (connectTowerComponent.TowerBelongPlayerID != currentPlayerComponent.PlayerID)
                        {
                            countConnectZone++;
                        }
                    }

                    if (countConnectZone > maxConnectZone)
                    {
                        guidSelectPotentiallyWeakTower.Add(towerComponent.GUID);
                        maxConnectZone = countConnectZone;
                    }
                }
            }
            
            SpawnUnit(FindZoneMinUnit(guidSelectPotentiallyWeakTower, currentPlayerComponent.PlayerID));
        }

        private string FindZoneMinUnit(List<string> selectTower, int playerID)
        {
            var selectGUID = "";
            var minCountUnit = 99;
            
            foreach (var towerGUID in selectTower)
            {
                var countUnit = _dataWorld.Select<UnitMapComponent>()
                    .Where<UnitMapComponent>(unit => unit.GUIDTower == towerGUID
                        && unit.PowerSolidPlayerID == playerID)
                    .Count();

                if (countUnit < minCountUnit)
                {
                    selectGUID = towerGUID;
                    minCountUnit = countUnit;
                }
            }

            return selectGUID;
        }
        
        private void SpawnUnit(string selectTowerGUID)
        {
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();

            ref var playerComponent = ref playerEntity.GetComponent<PlayerComponent>();
            ref var playerViewComponent = ref playerEntity.GetComponent<PlayerViewComponent>();
            playerComponent.UnitCount --;

            var towerEntity = _dataWorld.Select<TowerComponent>()
                .Where<TowerComponent>(tower => tower.GUID == selectTowerGUID)
                .SelectFirstEntity();

            var towerComponent = towerEntity.GetComponent<TowerComponent>();
            
            var unit = new InitUnitStruct 
            {
                KeyUnit = playerViewComponent.KeyCityVisual,
                UnitZone = towerComponent.SquadZonesMono[0],
                PlayerControl = PlayerControlEntity.PlayerControl,
                TargetPlayerID = playerComponent.PlayerID
            };
            CityAction.InitUnit?.Invoke(unit);
        }
        
        private void UpdateViewPlayer()
        {
            CityAction.UpdatePresencePlayerInCity?.Invoke();
            BoardGameUIAction.UpdateStatsPlayersCurrency?.Invoke();
        }

        public void Destroy()
        {
            AbilityAIAction.AddUnitMap -= AddUnit;
        }
    }
}