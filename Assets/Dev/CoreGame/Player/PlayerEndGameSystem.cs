using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.Player;
using CyberNet.Global;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class PlayerEndGameSystem : IDestroySystem
    {
        private DataWorld _dataWorld;

        public void Destroy()
        {
            var playerEntities = _dataWorld.Select<PlayerComponent>().GetEntities();

            foreach (var entity in playerEntities)
            {
                entity.Destroy();
            }
            
            _dataWorld.RemoveOneData<SelectPlayerData>();
        }
    }
}