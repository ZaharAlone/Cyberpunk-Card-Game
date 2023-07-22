using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core;
using ModulesFrameworkUnity;

namespace CyberNet.Meta
{
    [EcsSystem(typeof(MetaModule))]
    public class StartGameSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            StartGameAction.StartGameLocalVSAI += StartGameVSAI;
        }
        
        private void StartGameVSAI(string nameLeader)
        {
            ModulesUnityAdapter.world.InitModule<LocalGameModule>(true);
            ModulesUnityAdapter.world.InitModule<VSAIModule>(true);
            SelectAvatarFirstPlayer(nameLeader);
        }

        private void SelectAvatarFirstPlayer(string nameLeader)
        {
            _dataWorld.OneData<LeadersConfigData>().LeadersConfig.TryGetValue(nameLeader, out var leadersConfig);
            ref var playerView = ref _dataWorld.OneData<Player1ViewData>();
            playerView.AvatarKey = leadersConfig.imageAvatarLeader;
        }
    }
}