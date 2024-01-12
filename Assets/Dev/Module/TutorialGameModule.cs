using System.Collections.Generic;
using System.Threading.Tasks;
using CyberNet.Global;
using CyberNet.Tutorial;
using CyberNet.Tutorial.UI;
using ModulesFramework.Modules;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace EcsCore
{
    public class TutorialGameModule : EcsModule
    {
           private List<Object> _resource = new List<Object>();

        protected override async Task Setup()
        {
            var tasks = new List<Task>();

            var canvasTutorialUI = Load<GameObject>("TutorialUI", tasks);

            var alltask = Task.WhenAll(tasks.ToArray());
            await alltask;

            var canvasTutorialUIMono = Object.Instantiate(canvasTutorialUI.Result);
            world.CreateOneData(new TutorialData 
            { 
                TutorialUIMono  = canvasTutorialUIMono.GetComponent<TutorialUIMono>()
            });

            _resource.Add(canvasTutorialUIMono);
        }

        private Task<T> Load<T>(string name, List<Task> tasks)
        {
            var task = Addressables.LoadAssetAsync<T>(name).Task;
            tasks.Add(task);
            return task;
        }
        
        public override void OnDeactivate()
        {
            foreach (var item in _resource)
                Object.Destroy(item);
        }
    }
}