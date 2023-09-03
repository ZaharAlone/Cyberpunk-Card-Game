using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFrameworkUnity;
using System.Collections.Generic;
using CyberNet.Meta;
using UnityEngine;

namespace CyberNet.Core
{
    [EcsSystem(typeof(MetaModule))]
    public class PlayerViewSystem : IInitSystem
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            ref var playerView = ref _dataWorld.OneData<PlayerViewComponent>();
            playerView.Name = "Zakhar";
        }
    }
}