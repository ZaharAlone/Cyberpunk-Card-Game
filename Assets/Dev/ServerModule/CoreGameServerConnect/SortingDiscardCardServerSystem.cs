using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using System.Collections.Generic;
using UnityEngine;

namespace CyberNet.Core
{
    [EcsSystem(typeof(ServerModule))]
    public class SortingDiscardCardServerSystem : IInitSystem
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            GlobalCoreGameAction.SortingDiscardCard += SortingCard;
        }

        private void SortingCard(int targetPlayerID)
        {

        }
    }
}