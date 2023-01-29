using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using EcsCore;

namespace GameSystem.LifetimeData
{
    /// <summary>
    /// Controls time on Lifetime components
    /// After Lifetime ends add LifetimeEnd tag 
    /// </summary>
    [EcsSystem(typeof(GlobalModule))]
    public class TimeSystem : IRunSystem
    {
        private DataWorld _world;
        public void Run()
        {
            var q = _world.Select<Lifetime>();
            foreach (var entity in q.GetEntities())
            {
                ref var lt = ref entity.GetComponent<Lifetime>();
                lt.remain -= Time.deltaTime;
                if (lt.remain <= 0)
                    entity.RemoveComponent<Lifetime>();
            }
        }
    }
}