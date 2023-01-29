using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using GameSystem.LifetimeData;
using EcsCore;

namespace GameSystem.Delay
{
    [EcsSystem(typeof(GlobalModule))]
    public class DelayedSystem : IRunSystem
    {
        private DataWorld _world;
        
        public void Run()
        {
            var q = _world.Select<DelayData>()
                .Without<Lifetime>();
            foreach (var e in q.GetEntities())
            {
                ref var dd = ref e.GetComponent<DelayData>();
                if (dd.validate != null)
                {
                    if(dd.validate())
                        dd.delayedFun?.Invoke();
                }
                else
                {
                    dd.delayedFun?.Invoke();
                }
                e.Destroy();
            }
        }
    }
}