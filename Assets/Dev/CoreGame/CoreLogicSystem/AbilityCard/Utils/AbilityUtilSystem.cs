using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.AbilityCard;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityUtilSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AbilityCardUtilsAction.CalculateHowManyAbilitiesAvailableForSelection += CalculateHowManyAbilitiesAvailableForSelection;
        }

        private int CalculateHowManyAbilitiesAvailableForSelection(CardComponent cardComponent)
        {
            var abilityCardCount = 0;

            if (cardComponent.Ability_0.AbilityType != AbilityType.None)
            {
                if (CheckAbilityCard(cardComponent.Ability_0.AbilityType))
                    abilityCardCount++;
            }
            
            if (cardComponent.Ability_1.AbilityType != AbilityType.None)
            {
                if (CheckAbilityCard(cardComponent.Ability_1.AbilityType))
                    abilityCardCount++;
            }
            
            if (cardComponent.Ability_2.AbilityType != AbilityType.None && cardComponent.Ability_2.Condition == AbilityCondition.None)
            {
                if (CheckAbilityCard(cardComponent.Ability_2.AbilityType))
                    abilityCardCount++;
            }
            
            return abilityCardCount;
        }

        private bool CheckAbilityCard(AbilityType abilityType)
        {
            var currentRoundState = _dataWorld.OneData<RoundData>().CurrentRoundState;
            var abilityCardConfig = _dataWorld.OneData<CardsConfig>().AbilityCard;
            abilityCardConfig.TryGetValue(abilityType.ToString(), out var configCard);
            
            var isShow = false;
            if (currentRoundState == RoundState.Map)
            {
                if (configCard.VisualPlayingCardMap != VisualPlayingCardType.None)
                    isShow = true;
            }
            else
            {
                if (configCard.VisualPlayingCardArena != VisualPlayingCardType.None)
                    isShow = true;
            }
            return isShow;
        }

        public void Destroy()
        {
            AbilityCardUtilsAction.CalculateHowManyAbilitiesAvailableForSelection -= CalculateHowManyAbilitiesAvailableForSelection;
        }
    }
}