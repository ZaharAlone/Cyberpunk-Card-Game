using System;
using CyberNet.Core.AbilityCard;

namespace CyberNet.Core
{
    public static class AbilityCardUtilsAction
    {
        //Utils
        public static Func<CardComponent, int> CalculateHowManyAbilitiesAvailableForSelection;
        public static Func<AbilityType, bool> CheckAbilityIsPlayingOnMap;
    }
}