using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
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
        
        private void StartGameVSAI()
        {
            ModulesUnityAdapter.world.InitModule<LocalGameModule>(true);
            ModulesUnityAdapter.world.InitModule<VSAIModule>(true);    
        }
    }
}