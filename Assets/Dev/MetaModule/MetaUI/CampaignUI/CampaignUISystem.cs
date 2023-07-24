using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Meta;

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
            CampaignUIAction.BackMainMenu += BackMainMenu;
        }
        
        private void OpenCampaignUI()
        {
            //TO-DO Check Save game
            // if save clear => new game else view frame

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

        }

        private void ContinueCampaign()
        {

        }
    }
}