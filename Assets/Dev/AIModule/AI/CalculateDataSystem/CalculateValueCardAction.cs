using System;
namespace CyberNet.Core.Enemy
{
    public static class CalculateValueCardAction
    {
        public static Func<int, int> AttackAction;
        public static Func<int, int> TradeAction;
        public static Func<int, int> InfluenceAction;
        public static Func<int> DrawCardAction;
        public static Func<int> DestroyCardAction;
        public static Func<int> DiscardCardAction;
        public static Func<int> NoiseCardAction;
    }
}