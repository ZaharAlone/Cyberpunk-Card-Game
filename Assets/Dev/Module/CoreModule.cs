using EcsCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;
using BoardGame;
using BoardGame.Core;
using ModulesFramework.Modules;
using ModulesFrameworkUnity;
using ModulesFramework.Data;
using BoardGame.Core.UI;

namespace EcsCore
{
    public class CoreModule : EcsModule
    {
        private List<Object> _resource = new List<Object>();

        protected override async Task Setup()
        {
            var tasks = new List<Task>();

            var ui = Load<GameObject>("BoardGameUI", tasks);

            var alltask = Task.WhenAll(tasks.ToArray());
            await alltask;

            var uiObject = Object.Instantiate(ui.Result);
            world.CreateOneData(new UIData { UIGO = uiObject, UIMono = uiObject.GetComponent<BoardGameUIMono>() });

            _resource.Add(uiObject);
        }

        private Task<T> Load<T>(string name, List<Task> tasks)
        {
            var task = Addressables.LoadAssetAsync<T>(name).Task;
            tasks.Add(task);
            return task;
        }

        protected override Dictionary<Type, int> GetSystemsOrder()
        {
            return new Dictionary<Type, int>
            {
                { typeof(CardDistributionSystem), -10 },
                { typeof(HandUISystem), 0}
            };
        }

        public override void OnDeactivate()
        {
            foreach (var item in _resource)
                Object.Destroy(item);
        }
    }
}