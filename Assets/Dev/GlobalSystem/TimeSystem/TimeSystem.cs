using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;

namespace CyberNet.Global
{
    [EcsSystem(typeof(CoreModule))]
    public class TimeSystem : IRunSystem
    {
        private DataWorld _dataWorld;

        public void Run()
        {
            var timeEntities = _dataWorld.Select<TimeComponent>().GetEntities();

            foreach (var timeEntity in timeEntities)
            {
                ref var timeComponent = ref timeEntity.GetComponent<TimeComponent>();

                timeComponent.Time -= Time.deltaTime;
                if (timeComponent.Time <= 0)
                {
                    timeComponent.Action?.Invoke();
                    if (timeEntity.IsAlive())
                        timeEntity.RemoveComponent<TimeComponent>();
                }
            }
        }
    }
}