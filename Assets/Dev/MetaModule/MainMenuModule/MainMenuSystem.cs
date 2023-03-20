using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFrameworkUnity;
using UnityEngine;
using System;
using BoardGame.Server;

namespace BoardGame.Meta
{
    [EcsSystem(typeof(MetaModule))]
    public class MainMenuSystem : IInitSystem
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            MainMenuAction.StartGame += StartGame;
            MainMenuAction.ConnectServer += InitServer;
        }

        private void StartGame()
        {
            var menu = _dataWorld.OneData<MainMenuData>();
            menu.UI.SetActive(false);
            ModulesUnityAdapter.world.InitModule<BoardGameModule>(true);
        }

        private void InitServer()
        {
            ModulesUnityAdapter.world.InitModule<ServerModule>(true);
            ConnectServerAction.ConnectServer.Invoke();
        }
    }
}