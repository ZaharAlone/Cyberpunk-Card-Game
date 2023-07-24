using EcsCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;
using CyberNet;
using CyberNet.Core;
using ModulesFramework.Modules;
using ModulesFrameworkUnity;
using ModulesFramework.Data;
using CyberNet.Core.UI;

namespace EcsCore
{
    public class CoreModule : EcsModule
    {
        private List<Object> _resource = new List<Object>();

        protected override async Task Setup()
        {
            var tasks = new List<Task>();

            var canvasMainCore = Load<GameObject>("CoreGameUI", tasks);
            var canvasViewCard = Load<GameObject>("CanvasViewCard", tasks);

            var alltask = Task.WhenAll(tasks.ToArray());
            await alltask;

            var canvasMainCoreGO = Object.Instantiate(canvasMainCore.Result);
            var canvasMainMono = canvasMainCoreGO.GetComponent<BoardGameUIMono>();
            var canvasViewCardMono = Object.Instantiate(canvasViewCard.Result).GetComponent<ViewDeckCardUIMono>();
            world.CreateOneData(new UIData { UIGO = canvasMainCoreGO, UIMono = canvasMainMono, ViewDeckCard = canvasViewCardMono});

            _resource.Add(canvasMainCoreGO);
            _resource.Add(canvasViewCardMono.gameObject);
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
                { typeof(AnimationsCardInHand), 0},
                { typeof(EndLoadingSystem), 1000}
            };
        }

        public override void OnDeactivate()
        {
            foreach (var item in _resource)
                Object.Destroy(item);
        }
    }
}