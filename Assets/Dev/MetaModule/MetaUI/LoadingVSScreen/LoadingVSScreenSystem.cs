using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core;

namespace CyberNet.Meta
{
    [EcsSystem(typeof(MetaModule))]
    public class LoadingVSScreenSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            LoadingVSScreenAction.OpenLoadingVSScreen += OpenLoadingVSScreen;
            LoadingVSScreenAction.CloseLoadingVSScreen += CloseLoadingVSScreen;
        }

        //TODO: вернуть
        private void OpenLoadingVSScreen()
        {
            /*
            ref var playerView_1 = ref _dataWorld.OneData<PlayerViewComponent>();
            ref var playerView_2 = ref _dataWorld.OneData<Player2ViewData>();
            ref var uiVSScreen = ref _dataWorld.OneData<MetaUIData>().MetaUIMono.loadingVSScreenUIMono;
            ref var leadersConfigData = ref _dataWorld.OneData<LeadersConfigData>().LeadersConfig;
            ref var leadersView = ref _dataWorld.OneData<LeadersViewData>().LeadersView;
            
            leadersConfigData.TryGetValue(playerView_1.LeaderKey, out var playerConfig_1);
            leadersConfigData.TryGetValue(playerView_2.LeaderKey, out var playerConfig_2);
            leadersView.TryGetValue(playerConfig_1.ImageCardLeaders, out var player1CardSprite);
            leadersView.TryGetValue(playerConfig_2.ImageCardLeaders, out var player2CardSprite);

            uiVSScreen.SetLeader(player1CardSprite, player2CardSprite);
            uiVSScreen.OpenWindow();*/
        }
        
        private void CloseLoadingVSScreen()
        {
            _dataWorld.OneData<MetaUIData>().MetaUIMono.loadingVSScreenUIMono.CloseWindow();
        }
    }
}