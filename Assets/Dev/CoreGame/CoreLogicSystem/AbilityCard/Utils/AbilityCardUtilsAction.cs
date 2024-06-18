using System;
using CyberNet.Core.AbilityCard;

namespace CyberNet.Core
{
    public static class AbilityCardUtilsAction
    {
        //Utils
        public static Func<CardComponent, int> CalculateHowManyAbilitiesAvailableForSelection;
        public static Func<AbilityType, bool> CheckAbilityIsPlayingOnlyArena;
        public static Func<AbilityType, bool> CheckAbilityIsPlayingOnlyMap;
    }
}