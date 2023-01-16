using System;

namespace GameSystem.Delay
{
    public struct DelayData
    {
        public Action delayedFun;
        public Func<bool> validate;
    }
}