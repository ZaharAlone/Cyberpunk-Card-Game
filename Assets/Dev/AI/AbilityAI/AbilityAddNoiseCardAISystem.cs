using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.AI.Ability;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityAddNoiseCardAISystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AbilityAIAction.AddNoiseSelectPlayer += AddNoiseCard;
        }
        private void AddNoiseCard()
        {
            //TODO дописать AI добавление карты шума.
        }

        public void Destroy()
        {
            AbilityAIAction.AddNoiseSelectPlayer -= AddNoiseCard;
        }
    }
}