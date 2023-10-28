using System;
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
            CityAction.UpdatePlayerViewCity += UpdateTowerControlView;
        }
        
        private void ShowFirstBaseTower()
        {
            var entitiesTower = _dataWorld.Select<TowerComponent>()
                .With<FirstBasePlayerComponent>()
                .GetEntities();

            //TODO переписать
            foreach (var entityTower in entitiesTower)
            {
                ref var towerComponent = ref entityTower.GetComponent<TowerComponent>();
                //towerComponent.SelectTowerEffect.startColor = new Color32(255, 255, 255, 255);
                towerComponent.TowerMono.ActivateCollider();
            }
        }
        
        private void HideFirstBaseTower()
        {
            var entitiesTower = _dataWorld.Select<TowerComponent>()
                .GetEntities();

            //TODO переписать
            foreach (var entityTower in entitiesTower)
            {
                ref var towerComponent = ref entityTower.GetComponent<TowerComponent>();
                //towerComponent.SelectTowerEffect.startColor = new Color32(255, 255, 255, 25);
                towerComponent.TowerMono.DeactivateCollider();
            }
        }

        private void UpdateTowerControlView()
        {
            var entitiesTower = _dataWorld.Select<TowerComponent>()
                .With<FirstBasePlayerComponent>()
                .GetEntities();

            foreach (var entityTower in entitiesTower)
            {
                Debug.LogError("SetColor tower");
                ref var towerComponent = ref entityTower.GetComponent<TowerComponent>();
                var material = towerComponent.TowerMono.VisualEffectZone.materials[0];
                switch (towerComponent.PlayerIsBelong)
                {
                    case PlayerControlEnum.None:
                        material = SetFreeZone(material);
                        break;
                    case PlayerControlEnum.Neutral:
                        material = SetViewNeutralZoneControl(material);
                        break;
                    case PlayerControlEnum.Player:
                        material = SetViewPlayerZoneControl(material, towerComponent.TowerBelongPlyaerID);
                        break;
                }
                towerComponent.TowerMono.VisualEffectZone.materials[0] = material;
            }
        }

        private Material SetViewNeutralZoneControl(Material material)
        {
            material.SetColor("_Color", Color.black);
            material.SetFloat("_Thickness", -0.2f);
            material.SetFloat("_PowerEmission", 0f);
            return material;
        }

        private Material SetViewPlayerZoneControl(Material material, int playerID)
        {
            return material;
        }

        private Material SetFreeZone(Material material)
        {
            material.SetColor("_Color", Color.white);
            material.SetFloat("_Thickness", 0.5f);
            material.SetFloat("_PowerEmission", 0.2f);
            return material;
        }
    }
}