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
    public class AbilityDestroyCardAISystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AbilityAIAction.DestroyCard += DestroyCard;
        }

        private void DestroyCard(string guidCard)
        {
            //TODO дописать AI уничтожение карты.
        }

        public void Destroy()
        {
            AbilityAIAction.DestroyCard -= DestroyCard;
        }
    }
}