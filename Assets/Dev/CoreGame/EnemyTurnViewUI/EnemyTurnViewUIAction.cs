using System;

namespace CyberNet.Core.EnemyTurnView
{
    public static class EnemyTurnViewUIAction
    {
        public static Action<EnemyTurnActionType, string> ShowViewEnemyCard;
        public static Action ForceHideView;
    }
}