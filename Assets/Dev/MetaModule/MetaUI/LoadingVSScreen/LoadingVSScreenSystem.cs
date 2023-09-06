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

        //TODO: поправить визуал и логику загрузочного экрана
        private void OpenLoadingVSScreen()
        {
            ref var uiVSScreen = ref _dataWorld.OneData<MetaUIData>().MetaUIMono.loadingVSScreenUIMono;
            uiVSScreen.OpenWindow();
        }
        
        private void CloseLoadingVSScreen()
        {
            _dataWorld.OneData<MetaUIData>().MetaUIMono.loadingVSScreenUIMono.CloseWindow();
        }
    }
}