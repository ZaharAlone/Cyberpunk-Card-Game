using System;

namespace CyberNet.Core.EnemyTurnView
{
    public static class EnemyTurnViewUIAction
    {
        public static Action<EnemyTurnActionType, string> ShowViewEnemyCardCurrentPlayer;
        public static Action<EnemyTurnActionType, string, int> ShowViewEnemyCardSetPlayer;
        public static Action ForceHideView;
    }
}