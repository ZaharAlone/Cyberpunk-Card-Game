using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.Player;

namespace CyberNet.Core.AbilityCard
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityPlayerDiscardCardSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AbilityCardAction.PlayerDiscardCard += PlayerDiscardCard;
        }
        
        private void PlayerDiscardCard()
        {
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();

            ref var discardCardComponent = ref playerEntity.GetComponent<PlayerDiscardCardComponent>();
            
            
            RoundAction.StartTurn?.Invoke();
        }

        public void Destroy()
        {
            AbilityCardAction.PlayerDiscardCard -= PlayerDiscardCard;
        }
    }
}