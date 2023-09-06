using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using CyberNet.Core;
using CyberNet.Server;
using ModulesFrameworkUnity;
using Random = UnityEngine.Random;

namespace CyberNet.Meta.StartGame
{
    [EcsSystem(typeof(MetaModule))]
    public class StartGameSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            StartGameAction.StartLocalGame += StartLocalGame;
        }

        private void StartLocalGame()
        {
            LoadingVSScreenAction.OpenLoadingVSScreen?.Invoke();
            
            _dataWorld.InitModule<LocalGameModule>(true);
            _dataWorld.InitModule<VSAIModule>(true);
        }
        
        private void OnlineGame()
        {
            ConnectServer();
        }
        
        private void ConnectServer()
        {
            ModulesUnityAdapter.world.InitModule<ServerModule>(true);
            ConnectServerAction.ConnectServer.Invoke();
        }

    }
}