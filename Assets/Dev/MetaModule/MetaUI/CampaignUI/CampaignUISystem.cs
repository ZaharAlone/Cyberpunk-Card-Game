using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Meta;
using CyberNet.Meta.StartGame;
using CyberNet.SaveSystem;

namespace CyberNet.Core
{
    [EcsSystem(typeof(MetaModule))]
    public class CampaignUISystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            CampaignUIAction.OpenCampaignUI += OpenCampaignUI;
            CampaignUIAction.ContinueCampaign += ContinueCampaign;
            CampaignUIAction.NewCampaign += NewCampaign;
            CampaignUIAction.NewCampaignConfim += NewCampaignConfim;
            CampaignUIAction.BackMainMenu += BackMainMenu;
        }

        private void OpenCampaignUI()
        {
            ref var progressCompany = ref _dataWorld.OneData<SaveData>().ProgressCompany;
            if (progressCompany == ProgressCompany.None)
            {
                StartGameAction.StartCampaign?.Invoke();
                return;
            }
            
            ref var uiCampaign = ref _dataWorld.OneData<MetaUIData>().MetaUIMono.CampaignUIMono;
            uiCampaign.OpenWindow();
        }

        private void BackMainMenu()
        {
            MainMenuAction.OpenMainMenu?.Invoke();
            ref var uiCampaign = ref _dataWorld.OneData<MetaUIData>().MetaUIMono.CampaignUIMono;
            uiCampaign.CloseWindow();
        }

        private void NewCampaign()
        {
            ref var popupViewConfig = ref _dataWorld.OneData<PopupData>().PopupViewConfig;
            var popupView = new PopupConfimStruct();
            popupView.HeaderLoc = popupViewConfig.HeaderPopupNewCampaign;
            popupView.DescrLoc = popupViewConfig.DescrNewCampaign;
            popupView.ButtonConfimLoc = popupViewConfig.ConfimButtonPopupNewCampaign;
            popupView.ButtonCancelLoc = popupViewConfig.CancelButtonPopupNewCampaign;
            popupView.ButtonConfimAction = CampaignUIAction.NewCampaignConfim;
            PopupAction.ConfirmPopup?.Invoke(popupView);
        }

        private void NewCampaignConfim()
        {
            _dataWorld.OneData<SaveData>().ProgressCompany = ProgressCompany.None;
            StartGameAction.StartCampaign?.Invoke();
        }
        
        private void ContinueCampaign()
        {

        }
    }
}