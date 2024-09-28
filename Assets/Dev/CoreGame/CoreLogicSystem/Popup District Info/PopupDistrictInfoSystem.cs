using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using CyberNet.Core.Map;
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

        private void CheckOpenPopup(string guidDistrict)
        {
            var isShowPopup = _dataWorld.OneData<SettingsData>().GameSettings.IsShowDistrickPopup;
            
            if (!isShowPopup)
                return;
            
            var openPopupDistrictQuery = _dataWorld.Select<PopupDistrictInfoComponent>();

            if (openPopupDistrictQuery.Count() == 0)
            {
                var entity = _dataWorld.NewEntity();
                entity.AddComponent(new PopupDistrictInfoComponent { DistrictGUID = guidDistrict });
                OpenPopup(guidDistrict);
            }
            else
            {
                ref var popupComponent = ref openPopupDistrictQuery
                    .SelectFirstEntity()
                    .GetComponent<PopupDistrictInfoComponent>();
                
                if (popupComponent.DistrictGUID != guidDistrict)
                {
                    popupComponent.DistrictGUID = guidDistrict;
                    OpenPopup(guidDistrict);
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
            
            OpenPopup(openPopupDistrictComponent.DistrictGUID);
        }

        private void OpenPopup(string guidDistrict)
        {
            var districtComponent = _dataWorld.Select<DistrictComponent>()
                .Where<DistrictComponent>(district => district.GUID == guidDistrict)
                .SelectFirstEntity()
                .GetComponent<DistrictComponent>();

            var districtUI = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.PopupDistrictInfoUIMono;
            ref var cityVisual = ref _dataWorld.OneData<BoardGameData>().CitySO;
            
            if (districtComponent.PlayerControlEntity == PlayerControlEntity.PlayerControl)
            {
                var playerControlTowerEntity = _dataWorld.Select<PlayerComponent>()
                    .Where<PlayerComponent>(player => player.PlayerID == districtComponent.DistrictBelongPlayerID)
                    .SelectFirstEntity();
                var playerControlTowerViewComponent = playerControlTowerEntity.GetComponent<PlayerViewComponent>();

                cityVisual.UnitDictionary.TryGetValue(playerControlTowerViewComponent.KeyCityVisual, out var unitVisual);
                districtUI.SetFractionView(unitVisual.IconsUnit, unitVisual.ColorUnit, playerControlTowerViewComponent.Name);
            }
            else if (districtComponent.PlayerControlEntity == PlayerControlEntity.NeutralUnits)
            {
                cityVisual.UnitDictionary.TryGetValue("neutral_unit", out var unitVisual);
                districtUI.SetFractionView(unitVisual.IconsUnit, unitVisual.ColorUnit, cityVisual.DistrictNeutralLoc);
            }
            else
            {
                districtUI.SetFractionView(null, Color.white, cityVisual.DistrictClearLoc);
            }
            
            var districtConfigs = _dataWorld.OneData<BoardGameData>().DistrictConfig;
            districtConfigs.TryGetValue(districtComponent.Key, out var districtConfig);
            
            districtUI.OpenPopup(districtConfig.NameLoc, districtConfig.DescrLoc);
            SetViewBonus(districtComponent.Key);
        }

        private void SetViewBonus(string keyDistrict)
        {
            var districtUI = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.PopupDistrictInfoUIMono;
            var boardGameConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig;
            var districtConfig = _dataWorld.OneData<BoardGameData>().DistrictConfig;
            var currentDistrictBonus = districtConfig[keyDistrict].Bonus;

            var iconsBonus = boardGameConfig.CurrencyImage[currentDistrictBonus.Item];
            var colorBonus = boardGameConfig.CurrencyColor[currentDistrictBonus.Item];
            
            districtUI.SetBonus(iconsBonus, colorBonus, currentDistrictBonus.Value);
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