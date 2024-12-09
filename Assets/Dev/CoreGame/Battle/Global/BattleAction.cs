using System;
using System.Collections.Generic;
using CyberNet.Core.Battle.TacticsMode;
using ModulesFramework.Data;

namespace CyberNet.Core.Battle
{
    public static class BattleAction
    {
        public static Action StartBattleInMap;
        
        public static Action<string> EndMovePlayerToNewDistrict;
        public static Action EndMoveWithoutBattle;
        public static Action FinishBattle;
        
        public static Action<int> OpenTacticsScreen;
        public static Action CloseTacticsScreen;

        public static Action<int> SelectTacticsAI;
        public static Action SelectTacticsCardNeutralUnit;

        public static Func<int, int> CalculatePlayerMaxPower;
        public static Func<int, List<CardSelectTacticsPotential>> CalculatePlayerCardsPotential;

        public static Action CalculateResultBattle;

        public static Func<PowerKillDefenceDTO, Entity, PowerKillDefenceDTO> CalculatePlayerStatsInBattle;

        public static Action KillUnitInMapView;
        public static Action EndAnimationsKillUnitsInMap;
        public static Action ShowOtherPlayersBattleWinLoseInfo;

        public static Func<int, bool> CheckBattleFriendlyUnitsPresenceNeighboringDistrict;
    }
}