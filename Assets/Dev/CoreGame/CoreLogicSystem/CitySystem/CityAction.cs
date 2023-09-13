using System;
namespace CyberNet.Core.City
{
    public static class CityAction
    {
        public static Action ShowFirstBaseTower;
        public static Action HideFirstBaseTower;
        public static Action UpdatePlayerViewCity;
        public static Action ViewAllAvailableTower;

        public static Action UpdatePresencePlayerInCity;
        public static Action EnableNewPresencePlayerInCity;

        public static Action<InitUnitStruct> InitUnit;
        public static Action<string, int> ClearSolidPoint;
    }
}