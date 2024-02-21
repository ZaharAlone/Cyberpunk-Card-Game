using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using CyberNet.Core.City;
using CyberNet.Core.Player;
using CyberNet.Core.UI;
using CyberNet.Core.UI.PopupDistrictInfo;
using CyberNet.SaveSystem;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class PopupDistrictInfoSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            PopupDistrictInfoAction.OpenPopup += CheckOpenPopup;
            PopupDistrictInfoAction.ClosePopup += CheckClosePopup;
            PopupDistrictInfoAction.ForceUpdateViewCurrentPopup += ForceUpdateViewCurrentPopup;
        }

        private void CheckOpenPopup(string guidTower)
        {
            var isShowPopup = _dataWorld.OneData<SettingsData>().GameSettings.IsShowDistrickPopup;
            
            if (!isShowPopup)
                return;
            
            var openPopupDistrictQuery = _dataWorld.Select<PopupDistrictInfoComponent>();

            if (openPopupDistrictQuery.Count() == 0)
            {
                var entity = _dataWorld.NewEntity();
                entity.AddComponent(new PopupDistrictInfoComponent { TowerGUID = guidTower });
                OpenPopup(guidTower);
            }
            else
            {
                ref var popupComponent = ref openPopupDistrictQuery
                    .SelectFirstEntity()
                    .GetComponent<PopupDistrictInfoComponent>();
                
                if (popupComponent.TowerGUID != guidTower)
                {
                    popupComponent.TowerGUID = guidTower;
                    OpenPopup(guidTower);
                }
            }
        }

        private void CheckClosePopup()
        {
            var openPopupQuery = _dataWorld.Select<PopupDistrictInfoComponent>();

            if (openPopupQuery.Count() != 0)
            {
                openPopupQuery.SelectFirstEntity().Destroy();
                ClosePopup();
            }
        }
        
        private void ForceUpdateViewCurrentPopup()
        {
            var openPopupDistrictQuery = _dataWorld.Select<PopupDistrictInfoComponent>();
            if (openPopupDistrictQuery.Count() == 0)
                return;
            
            var openPopupDistrictComponent = openPopupDistrictQuery.SelectFirstEntity().GetComponent<PopupDistrictInfoComponent>();
            
            OpenPopup(openPopupDistrictComponent.TowerGUID);
        }

        private void OpenPopup(string guidTower)
        {
            var towerComponent = _dataWorld.Select<TowerComponent>()
                .Where<TowerComponent>(tower => tower.GUID == guidTower)
                .SelectFirstEntity()
                .GetComponent<TowerComponent>();

            var districtUI = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.PopupDistrictInfoUIMono;
            ref var cityVisual = ref _dataWorld.OneData<BoardGameData>().CitySO;
            
            if (towerComponent.PlayerControlEntity == PlayerControlEntity.PlayerControl)
            {
                var playerControlTowerEntity = _dataWorld.Select<PlayerComponent>()
                    .Where<PlayerComponent>(player => player.PlayerID == towerComponent.TowerBelongPlayerID)
                    .SelectFirstEntity();
                var playerControlTowerViewComponent = playerControlTowerEntity.GetComponent<PlayerViewComponent>();

                cityVisual.UnitDictionary.TryGetValue(playerControlTowerViewComponent.KeyCityVisual, out var unitVisual);
                districtUI.SetFractionView(unitVisual.IconsUnit, unitVisual.ColorUnit, playerControlTowerViewComponent.Name);
            }
            else if (towerComponent.PlayerControlEntity == PlayerControlEntity.NeutralUnits)
            {
                cityVisual.UnitDictionary.TryGetValue("neutral_unit", out var unitVisual);
                districtUI.SetFractionView(unitVisual.IconsUnit, unitVisual.ColorUnit, cityVisual.DistrictNeutralLoc);
            }
            else
            {
                districtUI.SetFractionView(null, Color.white, cityVisual.DistrictClearLoc);
            }
            
            var districtConfigs = _dataWorld.OneData<BoardGameData>().DistrictConfig;
            districtConfigs.TryGetValue(towerComponent.Key, out var districtConfig);
            
            districtUI.OpenPopup(districtConfig.NameLoc, districtConfig.DescrLoc);
        }
        
        private void ClosePopup()
        {
            var districtUI = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.PopupDistrictInfoUIMono;
            districtUI.ClosePopup();
        }

        public void Destroy()
        {
            PopupDistrictInfoAction.OpenPopup -= CheckOpenPopup;
            PopupDistrictInfoAction.ClosePopup -= CheckClosePopup;
            PopupDistrictInfoAction.ForceUpdateViewCurrentPopup -= ForceUpdateViewCurrentPopup;
        }
    }
}