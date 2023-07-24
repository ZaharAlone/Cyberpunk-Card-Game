using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;

namespace CyberNet.Meta
{
    [EcsSystem(typeof(MetaModule))]
    public class OnlineGameUISystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            OnlineGameUIAction.OpenOnlineGameUI += OpenOnlineGameUI;
            OnlineGameUIAction.FindMatch += FindMatch;
            OnlineGameUIAction.CallFriend += CallFriend;
            OnlineGameUIAction.BackMainMenu += BackMainMenu;
        }

        private void OpenOnlineGameUI()
        {
            ref var uiOnlineGame = ref _dataWorld.OneData<MetaUIData>().MetaUIMono.OnlineGameUIMono;
            uiOnlineGame.OpenWindow();
        }

        private void BackMainMenu()
        {
            MainMenuAction.OpenMainMenu?.Invoke();
            ref var uiOnlineGame = ref _dataWorld.OneData<MetaUIData>().MetaUIMono.OnlineGameUIMono;
            uiOnlineGame.CloseWindow();
        }
        
        private void FindMatch()
        {
            
        }
        
        private void CallFriend()
        {
            
        }
    }
}