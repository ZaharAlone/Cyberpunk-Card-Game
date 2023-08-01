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
            StartGameAction.StartGameLocalVSAI += StartGameVSAI;
            StartGameAction.StartGameLocalVSPlayer += StartGameLocalVSPlayer;
        }

        private void StartGameVSAI(string nameLeader)
        {
            SelectAvatarPlayer(nameLeader, PlayerEnum.Player1);
            SelectViewBot();
            LoadingVSScreenAction.OpenLoadingVSScreen?.Invoke();
            
            _dataWorld.InitModule<LocalGameModule>(true);
            _dataWorld.InitModule<VSAIModule>(true);
        }

        private void StartGameLocalVSPlayer(string nameLeaderPlayer1, string nameLeaderPlayer2)
        {
            _dataWorld.InitModule<LocalGameModule>(true);
            _dataWorld.InitModule<PassAndPlayModule>(true);
            SelectAvatarPlayer(nameLeaderPlayer1, PlayerEnum.Player1);
            SelectAvatarPlayer(nameLeaderPlayer2, PlayerEnum.Player2);
        }
        
        private void SelectAvatarPlayer(string nameLeader, PlayerEnum targetPlayer)
        {
            _dataWorld.OneData<LeadersConfigData>().LeadersConfig.TryGetValue(nameLeader, out var leadersConfig);

            if (targetPlayer == PlayerEnum.Player1)
            {
                ref var playerView = ref _dataWorld.OneData<Player1ViewData>();
                playerView.LeaderKey = nameLeader;
                playerView.AvatarKey = leadersConfig.imageAvatarLeader;   
            }
            else if (targetPlayer == PlayerEnum.Player2)
            {
                ref var playerView = ref _dataWorld.OneData<Player2ViewData>();
                playerView.LeaderKey = nameLeader;
                playerView.AvatarKey = leadersConfig.imageAvatarLeader;  
            }
        }

        private void SelectViewBot()
        {
            ref var leadersConfigData = ref _dataWorld.OneData<LeadersConfigData>().LeadersConfig;
            var randomIndex = Random.Range(0, leadersConfigData.Count);

            var leadersConfig = leadersConfigData.ElementAt(randomIndex).Value;
            
            ref var playerView = ref _dataWorld.OneData<Player2ViewData>();
            playerView.LeaderKey = leadersConfig.Name;
            playerView.Name =  I2.Loc.LocalizationManager.GetTranslation(leadersConfig.NameLoc);
            playerView.AvatarKey = leadersConfig.imageAvatarLeader;
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