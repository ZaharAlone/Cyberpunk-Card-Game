using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFrameworkUnity;
using System.Collections.Generic;
using UnityEngine;

namespace BoardGame.Core
{
    [EcsSystem(typeof(MetaModule))]
    public class PlayerViewSystem : IInitSystem
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            ref var playerView = ref _dataWorld.OneData<Player1ViewData>();
            playerView.AvatarKey = "avatar_pirate";
            playerView.Name = "Zakhar";
        }
    }
}