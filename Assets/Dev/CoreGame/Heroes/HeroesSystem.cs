using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;

namespace CyberNet.Core.Heroes
{
    [EcsSystem(typeof(CoreModule))]
    public class HeroesSystem : IInitSystem, IRunSystem
    {
        private DataWorld _dataWorld;

        public void Init() { }

        public void Run() { }
    }
}