using System;
using ModulesFramework.Data;
using GameSystem.LifetimeData;

namespace GameSystem.Delay
{
    public static class DelayedFactory
    {
        public static int Create(DataWorld world, float delay, Action delayedAction, Func<bool> validate = null)
        {
            return world.NewEntity()
                .AddComponent(new Lifetime { remain = delay })
                .AddComponent(new DelayData
                {
                    delayedFun = delayedAction,
                    validate = validate
                })
                .Id;
        }
    }
}