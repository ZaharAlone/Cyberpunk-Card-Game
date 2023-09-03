using EcsCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;
using CyberNet.Core;
using CyberNet.Core.PauseUI;
using ModulesFramework.Modules;
using CyberNet.Core.UI;
using CyberNet.Global;

namespace EcsCore
{
    public class CoreModule : EcsModule
    {
        private List<Object> _resource = new List<Object>();

        protected override async Task Setup()
        {
            var tasks = new List<Task>();

            var canvasMainCore = Load<GameObject>("CoreGameUI", tasks);
            var pauseUI = Load<GameObject>("PauseUI", tasks);
            var canvasViewCard = Load<GameObject>("ViewCardUI", tasks);

            var alltask = Task.WhenAll(tasks.ToArray());
            await alltask;

            var canvasMainCoreGO = Object.Instantiate(canvasMainCore.Result);
            var canvasMainMono = canvasMainCoreGO.GetComponent<BoardGameUIMono>();
            var canvasViewCardMono = Object.Instantiate(canvasViewCard.Result).GetComponent<ViewDeckCardUIMono>();
            var pauseUIGO = Object.Instantiate(pauseUI.Result);
            var pauseUIMono = pauseUIGO.GetComponent<PauseGameUIMono>();
            world.CreateOneData(new CoreGameUIData 
            { 
                UIGO = canvasMainCoreGO, 
                BoardGameUIMono = canvasMainMono, 
                ViewDeckCard = canvasViewCardMono, 
                PauseGameUIMono = pauseUIMono
                
            });

            _resource.Add(canvasMainCoreGO);
            _resource.Add(canvasViewCardMono.gameObject);
            _resource.Add(pauseUIGO);
            
            ModuleAction.ActivateCoreModule?.Invoke();
        }

        private Task<T> Load<T>(string name, List<Task> tasks)
        {
            var task = Addressables.LoadAssetAsync<T>(name).Task;
            tasks.Add(task);
            return task;
        }
        //TODO: вернуть
        protected override Dictionary<Type, int> GetSystemsOrder()
        {
            return new Dictionary<Type, int>
            {
                { typeof(CardDistributionSystem), -10 },
               // { typeof(AnimationsFanCardInHandSystem), 0},
                { typeof(EndLoadingSystem), 1000}
            };
        }

        public override void OnDeactivate()
        {
            foreach (var item in _resource)
                Object.Destroy(item);
            
            ModuleAction.DeactivateCoreModule?.Invoke();
        }
    }
}