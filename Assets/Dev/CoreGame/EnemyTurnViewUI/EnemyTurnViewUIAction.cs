using System;
namespace CyberNet.Core.EnemyTurnView
{
    public static class EnemyTurnViewUIAction
    {
        public static Action<string> PlayingCardShowView;
        public static Action<string> PurchaseCardShowView;

        public static Action HideView;
    }
}