using CyberNet.Core.Player;
using CyberNet.Core.UI.PopupDistrictInfo;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace CyberNet.Core.City
{
    [EcsSystem(typeof(CoreModule))]
    public class CityVisualControlSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            CityAction.ShowFirstBaseTower += ShowFirstBaseTower;
            CityAction.HideFirstBaseTower += HideFirstBaseTower;
            CityAction.UpdatePlayerViewCity += UpdateTowerControlView;

            CityAction.EnableInteractiveTower += EnableInteractiveTower;
            CityAction.DisableInteractiveTower += DisableInteractiveTower;
        }
        
        private void ShowFirstBaseTower()
        {
            var entitiesTowerFirstBase = _dataWorld.Select<TowerComponent>()
                .With<FirstBasePlayerComponent>()
                .GetEntities();


            foreach (var entityTower in entitiesTowerFirstBase)
            {
                ref var towerComponent = ref entityTower.GetComponent<TowerComponent>();
                towerComponent.TowerMono.OnInteractiveTower();
                towerComponent.TowerMono.OpenInteractiveZoneVisualEffect();
            }
        }

        private void HideFirstBaseTower()
        {
            var entitiesTower = _dataWorld.Select<TowerComponent>().GetEntities();

            foreach (var entityTower in entitiesTower)
            {
                ref var towerComponent = ref entityTower.GetComponent<TowerComponent>();
                towerComponent.TowerMono.OffInteractiveTower();
                towerComponent.TowerMono.CloseInteractiveZoneVisualEffect();
            }
        }

        private void UpdateTowerControlView()
        {
            var entitiesTower = _dataWorld.Select<TowerComponent>()
                .GetEntities();

            foreach (var entityTower in entitiesTower)
            {
                ref var towerComponent = ref entityTower.GetComponent<TowerComponent>();
                var material = towerComponent.TowerMono.VisualEffectZone.materials[0];
                
                switch (towerComponent.PlayerControlEntity)
                {
                    case PlayerControlEntity.None:
                        material = SetFreeZone(material);
                        break;
                    case PlayerControlEntity.NeutralUnits:
                        material = SetViewNeutralZoneControl(material);
                        break;
                    case PlayerControlEntity.PlayerControl:
                        material = SetViewPlayerZoneControl(material, towerComponent.TowerBelongPlayerID);
                        break;
                }
                
                towerComponent.TowerMono.VisualEffectZone.materials[0] = material;
            }
            
            PopupDistrictInfoAction.ForceUpdateViewCurrentPopup?.Invoke();
        }

        private Material SetViewNeutralZoneControl(Material material)
        {
            material.SetColor("_Color", Color.white);
            material.SetFloat("_Thickness", 0.5f);
            material.SetFloat("_PowerEmission", 0.5f);
            
            return material;
        }

        private Material SetViewPlayerZoneControl(Material material, int playerID)
        {
            var entityPlayer = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player => player.PlayerID == playerID)
                .SelectFirstEntity();

            var playerViewComponent = entityPlayer.GetComponent<PlayerViewComponent>();
            ref var cityVisual = ref _dataWorld.OneData<BoardGameData>().CitySO;
            cityVisual.UnitDictionary.TryGetValue(playerViewComponent.KeyCityVisual, out var playerUnitVisual);
            
            material.SetColor("_Color", playerUnitVisual.ColorUnit);
            material.SetFloat("_Thickness", 0.5f);
            material.SetFloat("_PowerEmission", 0.5f);
            
            return material;
        }

        private Material SetFreeZone(Material material)
        {
            material.SetColor("_Color", Color.black);
            material.SetFloat("_Thickness", 0.2f);
            material.SetFloat("_PowerEmission", 0f);
            return material;
        }
        
        private void EnableInteractiveTower(string towerGuid)
        {
            var towerEntity = _dataWorld.Select<TowerComponent>()
                .Where<TowerComponent>(tower => tower.GUID == towerGuid)
                .SelectFirstEntity();
            var towerComponent = towerEntity.GetComponent<TowerComponent>();
            towerComponent.TowerMono.OnInteractiveTower();
            towerComponent.TowerMono.OpenInteractiveZoneVisualEffect();
        }
        private void DisableInteractiveTower(string towerGuid)
        {
            var towerEntity = _dataWorld.Select<TowerComponent>()
                .Where<TowerComponent>(tower => tower.GUID == towerGuid)
                .SelectFirstEntity();
            var towerComponent = towerEntity.GetComponent<TowerComponent>();
            towerComponent.TowerMono.OffInteractiveTower();
            towerComponent.TowerMono.CloseInteractiveZoneVisualEffect();
        }

        public void Destroy()
        {
            CityAction.ShowFirstBaseTower -= ShowFirstBaseTower;
            CityAction.HideFirstBaseTower -= HideFirstBaseTower;
            CityAction.UpdatePlayerViewCity -= UpdateTowerControlView;

            CityAction.EnableInteractiveTower -= EnableInteractiveTower;
            CityAction.DisableInteractiveTower -= DisableInteractiveTower;
        }
    }
}