using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using EcsCore;

namespace GameSystem.Tweens
{
    [EcsSystem(typeof(GlobalModule))]
    public class TweenSystem : IRunSystem
    {
        private DataWorld _world;

        public void Run()
        {
            var q = _world.Select<TweenData>();
            foreach (var entity in q.GetEntities())
            {
                ref var data = ref entity.GetComponent<TweenData>();
                if (data.validate != null)
                {
                    if (!data.validate())
                    {
                        entity.Destroy();
                        continue;
                    }
                }

                data.remain -= Time.deltaTime;
                if (data.remain <= 0) data.remain = 0;
                data.update?.Invoke(data.remain);
                if (data.remain == 0)
                {
                    data.onEnd?.Invoke();
                    entity.Destroy();
                }
            }
        }
    }
}