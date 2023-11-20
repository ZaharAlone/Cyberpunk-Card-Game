using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using System.Collections.Generic;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.Player;
using UnityEngine;

namespace CyberNet.Core.City
{
    [EcsSystem(typeof(CoreModule))]
    public class CityGameMechanicsSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            //CityAction.UpdateCanInteractiveMap += UpdateCanInteractiveMap;
            CityAction.ShowWherePlayerCanMove += ShowWherePlayerCanMove;
            CityAction.ShowWherePlayerCanMoveFrom += ShowWherePlayerCanMoveFrom;
            CityAction.ShowWherePlayerCanAddUnit += ShowWherePlayerCanAddUnit;
        }
        private void ShowWherePlayerCanAddUnit()
        {
            DeactivateAllTower();
            
            var towerEntities = _dataWorld.Select<TowerComponent>().GetEntities();
            
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();
            var playerComponent = playerEntity.GetComponent<PlayerComponent>();

            foreach (var towerEntity in towerEntities)
            {
                var towerComponent = towerEntity.GetComponent<TowerComponent>();

                if (towerComponent.PlayerIsBelong == PlayerControlEnum.Player
                    && towerComponent.TowerBelongPlyaerID == playerComponent.PlayerID)
                {
                    towerComponent.TowerMono.ActivateCollider();
                    towerComponent.TowerMono.OpenInteractiveZoneVisualEffect();
                }
                else
                {
                    towerComponent.TowerMono.DeactivateCollider();
                    towerComponent.TowerMono.CloseInteractiveZoneVisualEffect();
                }
            }
        }
        
        /*
        private void UpdateCanInteractiveMap()
        {
            var actionData = _dataWorld.OneData<ActionCardData>();
            var towerEntities = _dataWorld.Select<TowerComponent>().GetEntities();

            if (valueAttack == 0)
            {
                foreach (var entity in towerEntities)
                {
                    var towerMono = entity.GetComponent<TowerComponent>().TowerMono;
                    towerMono.DeactivateCollider();
                    towerMono.CloseInteractiveZoneVisualEffect();
                }
            }
            else
            {
                var playerEntity = _dataWorld.Select<PlayerComponent>()
                    .With<CurrentPlayerComponent>()
                    .SelectFirstEntity();
                var playerComponent = playerEntity.GetComponent<PlayerComponent>();

                foreach (var towerEntity in towerEntities)
                {
                    var towerComponent = towerEntity.GetComponent<TowerComponent>();

                    if (towerComponent.PlayerIsBelong == PlayerControlEnum.Player
                        && towerComponent.TowerBelongPlyaerID == playerComponent.PlayerID)
                    {
                        towerComponent.TowerMono.ActivateCollider();
                        towerComponent.TowerMono.OpenInteractiveZoneVisualEffect();
                    }
                    else
                    {
                        towerComponent.TowerMono.DeactivateCollider();
                        towerComponent.TowerMono.CloseInteractiveZoneVisualEffect();
                    }
                }
            }
        }*/
        
        /// <summary>
        /// Активирует зоны на которые игрок может передвинуть своего юнита
        /// </summary>
        private void ShowWherePlayerCanMove()
        {
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();

            var playerComponent = playerEntity.GetComponent<PlayerComponent>();

            var towerEntities = _dataWorld.Select<TowerComponent>()
                .Where<TowerComponent>(tower => tower.PlayerIsBelong == PlayerControlEnum.Player
                    && tower.TowerBelongPlyaerID == playerComponent.PlayerID)
                .GetEntities();

            foreach (var towerEntity in towerEntities)
            {
                var towerComponent = towerEntity.GetComponent<TowerComponent>();
                towerComponent.TowerMono.ActivateCollider();
                towerComponent.TowerMono.OpenInteractiveZoneVisualEffect();

                foreach (var towerConnect in towerComponent.TowerMono.ZoneConnect)
                {
                    towerConnect.ActivateCollider();
                    towerConnect.OpenInteractiveZoneVisualEffect();
                }
            }
        }
        
        /// <summary>
        /// Активирует зоны с которых игрок может передвинуть своих юнитов в целевую точку
        /// </summary>
        private void ShowWherePlayerCanMoveFrom(string GUIDTower)
        {
            DeactivateAllTower();
            
            var towerEntity = _dataWorld.Select<TowerComponent>()
                .Where<TowerComponent>(tower => tower.GUID == GUIDTower)
                .SelectFirstEntity();

            var towerComponent = towerEntity.GetComponent<TowerComponent>();

            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();

            var playerComponent = playerEntity.GetComponent<PlayerComponent>();
            foreach (var towerConnect in towerComponent.TowerMono.ZoneConnect)
            {
                var towerConnectEntity = _dataWorld.Select<TowerComponent>()
                    .Where<TowerComponent>(tower => tower.GUID == towerConnect.GUID)
                    .SelectFirstEntity();

                var towerConnectComponent = towerConnectEntity.GetComponent<TowerComponent>();
                if (towerConnectComponent.PlayerIsBelong == PlayerControlEnum.Player
                    && towerConnectComponent.TowerBelongPlyaerID == playerComponent.PlayerID)
                {
                    towerConnectComponent.TowerMono.OpenInteractiveZoneVisualEffect();
                }
            }
        }

        private void DeactivateAllTower()
        {
            var towerEntities = _dataWorld.Select<TowerComponent>().GetEntities();
            foreach (var towerEntity in towerEntities)
            {
                var towerComponent = towerEntity.GetComponent<TowerComponent>();
                towerComponent.TowerMono.DeactivateCollider();
                towerComponent.TowerMono.CloseInteractiveZoneVisualEffect();
            }
        }
    }
}