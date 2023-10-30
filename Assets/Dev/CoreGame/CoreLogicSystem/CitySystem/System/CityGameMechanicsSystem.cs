using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using System.Collections.Generic;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.Player;

namespace CyberNet.Core.City
{
    [EcsSystem(typeof(CoreModule))]
    public class CityGameMechanicsSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            CityAction.UpdateCanInteractiveMap += UpdateCanInteractiveMap;
            CityAction.ShowWhereIsMovePlayer += ShowWhereIsMovePlayer;
        }

        private void UpdateCanInteractiveMap()
        {
            var actionData = _dataWorld.OneData<ActionCardData>();
            var valueAttack = actionData.TotalAttack - actionData.SpendAttack;
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
        }
        
        /// <summary>
        /// Активирует зоны на которые игрок может передвинуть своего юнита
        /// </summary>
        private void ShowWhereIsMovePlayer()
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
    }
}