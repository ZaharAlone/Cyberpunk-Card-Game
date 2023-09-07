using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Global;

namespace CyberNet.Core.EnemyPassport
{
    [EcsSystem(typeof(CoreModule))]
    public class EnemyPassportSystem : IPreInitSystem, IInitSystem
    {
        private DataWorld _dataWorld;
        
        public void PreInit()
        {
            
        }

        public void Init()
        {
            
        }
    }
}