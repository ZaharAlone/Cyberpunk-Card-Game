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
            AbilityCardUtilsAction.CheckAbilityIsPlayingOnMap += CheckAbilityIsPlayingOnMap;
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
            
            return abilityCardCount;
        }

        private bool CheckAbilityCard(AbilityType abilityType)
        {
            var currentRoundState = _dataWorld.OneData<RoundData>().CurrentGameStateMapVSArena;
            var abilityCardConfig = _dataWorld.OneData<CardsConfig>().AbilityCard;
            abilityCardConfig.TryGetValue(abilityType.ToString(), out var configCard);
            
            var isShow = false;
            if (currentRoundState == GameStateMapVSArena.Map)
            {
                if (configCard.VisualPlayingCard != VisualPlayingCardType.None)
                    isShow = true;
            }
            else
            {
                if (configCard.VisualPlayingCard != VisualPlayingCardType.None)
                    isShow = true;
            }
            return isShow;
        }
        
        private bool CheckAbilityIsPlayingOnMap(AbilityType abilityType)
        {
            var abilityCardConfig = _dataWorld.OneData<CardsConfig>().AbilityCard;
            abilityCardConfig.TryGetValue(abilityType.ToString(), out var configCard);

            return configCard.VisualPlayingCard != VisualPlayingCardType.Battle;
        }

        public void Destroy()
        {
            AbilityCardUtilsAction.CalculateHowManyAbilitiesAvailableForSelection -= CalculateHowManyAbilitiesAvailableForSelection;
            AbilityCardUtilsAction.CheckAbilityIsPlayingOnMap -= CheckAbilityIsPlayingOnMap;
        }
    }
}