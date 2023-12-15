using System;
using System.Collections.Generic;

namespace CyberNet.Core.City
{
    public static class CityAction
    {
        public static Action ShowFirstBaseTower;
        public static Action HideFirstBaseTower;

        public static Action<string> EnableInteractiveTower;
        public static Action<string> DisableInteractiveTower;
        
        public static Action UpdatePlayerViewCity;
        public static Action ShowWherePlayerCanMove;
        public static Action<string> ShowWherePlayerCanMoveFrom;
        public static Action ViewAllAvailableTower;
        
        public static Action<int> ShowWhereZoneToPlayerID;
        public static Action<List<int>> ShowManyZonePlayerInMap;

        public static Action UpdatePresencePlayerInCity;
        public static Action EnableNewPresencePlayerInCity;
        public static Action UpdateCanInteractiveMap;

        public static Action<InitUnitStruct> InitUnit;
        public static Action<string, int> AttackSolidPoint;
        public static Action<string> SelectTower;
        public static Action<string> SelectUnit;
    }
}