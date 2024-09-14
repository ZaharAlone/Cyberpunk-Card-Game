using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.AI.Ability;
using CyberNet.Core.Arena.SelectZone;
using CyberNet.Core.BezierCurveNavigation;
using CyberNet.Global;
using Input;

namespace CyberNet.Core.AbilityCard
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityThrowGrenadeSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AbilityCardAction.ThrowGrenade += ThrowGrenade;
        }
        
        private void ThrowGrenade(string guidCard)
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();

            if (roundData.playerOrAI != PlayerOrAI.Player)
            {
                AbilityAIAction.ThrowGrenade?.Invoke(guidCard);
                return;
            }
            
            Debug.LogError("Grenade ability");
            AbilityPopupUISystemAction.OpenPopupAbilityTargetInfo?.Invoke(AbilityType.Grenade, 0, false);
            SelectZoneArenaAction.EnableSelectZone?.Invoke();
            InputAction.LeftMouseButtonClick += ClickMouse;
        }
        
        private void ClickMouse()
        {
            //SelectZoneArenaAction.DisableSelectZone?.Invoke();
        }

        public void Destroy() { }
    }
}