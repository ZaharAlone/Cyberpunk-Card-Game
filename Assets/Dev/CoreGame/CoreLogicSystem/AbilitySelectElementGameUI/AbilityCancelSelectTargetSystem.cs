using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.UI;

namespace CyberNet.Core.AbilityCard
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityCancelSelectTargetSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AbilitySelectElementAction.CancelSelect += CancelSelect;
        }
        
        private void CancelSelect()
        {
            ref var uiActionSelectCard = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.AbilitySelectElementUIMono;
            uiActionSelectCard.CloseWindow();
            
            var entitySelectAbilityTarget = _dataWorld.Select<CardComponent>()
                .With<AbilitySelectElementComponent>()
                .SelectFirstEntity();
            ref var selectAbility = ref entitySelectAbilityTarget.GetComponent<AbilitySelectElementComponent>().AbilityCard;
            
            switch (selectAbility.AbilityType)
            {
                case AbilityType.DestroyCard:
                    break;
                case AbilityType.CloneCard:
                    break;
                case AbilityType.DestroyTradeCard:
                    break;
                case AbilityType.SwitchNeutralSquad:
                    break;
                case AbilityType.SwitchEnemySquad:
                    break;
                case AbilityType.DestroyNeutralSquad:
                    break;
                case AbilityType.DestroySquad:
                    break;
                case AbilityType.SquadMove:
                    break;
                case AbilityType.SetIce:
                    break;
                case AbilityType.DestroyIce:
                    break;
                case AbilityType.EnemyDiscardCard:
                    AbilitySelectElementAction.CancelSelectPlayer?.Invoke();
                    AbilityCardAction.CancelDiscardCard?.Invoke();
                    break;
            }
            
            //TODO Return card in hand, break animation move table card
            
            entitySelectAbilityTarget.RemoveComponent<AbilitySelectElementComponent>();
        }
    }
}