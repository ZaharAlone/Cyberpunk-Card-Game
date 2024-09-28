using CyberNet.Core.Player;
using CyberNet.Core.UI.PopupDistrictInfo;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace CyberNet.Core.Map
{
    [EcsSystem(typeof(CoreModule))]
    public class CityVisualControlSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            CityAction.ShowFirstBaseTower += ShowFirstBaseTower;
            CityAction.HideFirstBaseTower += HideFirstBaseTower;
            CityAction.UpdateTowerControlView += UpdateTowerControlView;

            CityAction.EnableInteractiveTower += EnableInteractiveTower;
            CityAction.DisableInteractiveTower += DisableInteractiveTower;
        }
        
        private void ShowFirstBaseTower()
        {
            var entitiesTowerFirstBase = _dataWorld.Select<DistrictComponent>()
                .With<FirstBasePlayerComponent>()
                .GetEntities();


            foreach (var entityTower in entitiesTowerFirstBase)
            {
                ref var towerComponent = ref entityTower.GetComponent<DistrictComponent>();
                towerComponent.DistrictMono.OnInteractiveTower();
                towerComponent.DistrictMono.OpenInteractiveZoneVisualEffect();
            }
        }

        private void HideFirstBaseTower()
        {
            var entitiesTower = _dataWorld.Select<DistrictComponent>().GetEntities();

            foreach (var entityTower in entitiesTower)
            {
                ref var towerComponent = ref entityTower.GetComponent<DistrictComponent>();
                towerComponent.DistrictMono.OffInteractiveTower();
                towerComponent.DistrictMono.CloseInteractiveZoneVisualEffect();
            }
        }

        private void UpdateTowerControlView()
        {
            var entitiesTower = _dataWorld.Select<DistrictComponent>()
                .GetEntities();

            foreach (var entityTower in entitiesTower)
            {
                ref var towerComponent = ref entityTower.GetComponent<DistrictComponent>();
                var material = towerComponent.DistrictMono.VisualEffectZone.materials[0];
                
                switch (towerComponent.PlayerControlEntity)
                {
                    case PlayerControlEntity.None:
                        material = SetFreeZone(material);
                        break;
                    case PlayerControlEntity.NeutralUnits:
                        material = SetViewNeutralZoneControl(material);
                        break;
                    case PlayerControlEntity.PlayerControl:
                        material = SetViewPlayerZoneControl(material, towerComponent.DistrictBelongPlayerID);
                        break;
                }
                
                towerComponent.DistrictMono.VisualEffectZone.materials[0] = material;
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
            var towerEntity = _dataWorld.Select<DistrictComponent>()
                .Where<DistrictComponent>(tower => tower.GUID == towerGuid)
                .SelectFirstEntity();
            var towerComponent = towerEntity.GetComponent<DistrictComponent>();
            towerComponent.DistrictMono.OnInteractiveTower();
            towerComponent.DistrictMono.OpenInteractiveZoneVisualEffect();
        }
        private void DisableInteractiveTower(string towerGuid)
        {
            var towerEntity = _dataWorld.Select<DistrictComponent>()
                .Where<DistrictComponent>(tower => tower.GUID == towerGuid)
                .SelectFirstEntity();
            var towerComponent = towerEntity.GetComponent<DistrictComponent>();
            towerComponent.DistrictMono.OffInteractiveTower();
            towerComponent.DistrictMono.CloseInteractiveZoneVisualEffect();
        }

        public void Destroy()
        {
            CityAction.ShowFirstBaseTower -= ShowFirstBaseTower;
            CityAction.HideFirstBaseTower -= HideFirstBaseTower;
            CityAction.UpdateTowerControlView -= UpdateTowerControlView;

            CityAction.EnableInteractiveTower -= EnableInteractiveTower;
            CityAction.DisableInteractiveTower -= DisableInteractiveTower;
        }
    }
}