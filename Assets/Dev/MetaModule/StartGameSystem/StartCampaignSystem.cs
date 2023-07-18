using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;

namespace CyberNet.Core
{
    [EcsSystem(typeof(MetaModule))]
    public class StartCampaignSystem : IInitSystem, IRunSystem
    {
        private DataWorld _dataWorld;

        public void Init() { }

        public void Run() { }
    }
}