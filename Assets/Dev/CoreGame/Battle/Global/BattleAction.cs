using System;

namespace CyberNet.Core.Battle
{
    public static class BattleAction
    {
        public static Action<string> EndMovePlayerToNewDistrict;
        public static Action FinishBattle;
        
        public static Action OpenTacticsScreen;

        public static Action OnClickStartBattle;
    }
}