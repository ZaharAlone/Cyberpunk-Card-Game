using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace CyberNet.Core.City
{
    [EcsSystem(typeof(CoreModule))]
    public class CityVisualControlSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            CityAction.ShowFirstBaseTower += ShowFirstBaseTower;
            CityAction.HideFirstBaseTower += HideFirstBaseTower;
        }

        private void ShowFirstBaseTower()
        {
            var entitiesTower = _dataWorld.Select<TowerComponent>()
                .With<FirstBasePlayer>()
                .GetEntities();

            foreach (var entityTower in entitiesTower)
            {
                ref var towerComponent = ref entityTower.GetComponent<TowerComponent>();
                towerComponent.SelectTowerEffect.startColor = new Color32(255, 255, 255, 255);
                towerComponent.TowerMono.ActivateCollider();
            }
        }
        
        private void HideFirstBaseTower()
        {
            var entitiesTower = _dataWorld.Select<TowerComponent>()
                .GetEntities();

            foreach (var entityTower in entitiesTower)
            {
                ref var towerComponent = ref entityTower.GetComponent<TowerComponent>();
                towerComponent.SelectTowerEffect.startColor = new Color32(255, 255, 255, 25);
                towerComponent.TowerMono.DeactivateCollider();
            }
        }
    }
}