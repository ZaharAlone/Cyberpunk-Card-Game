using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Meta;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class EndLoadingSystem : IInitSystem
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            LoadingVSScreenAction.CloseLoadingVSScreen?.Invoke();
        }
    }
}