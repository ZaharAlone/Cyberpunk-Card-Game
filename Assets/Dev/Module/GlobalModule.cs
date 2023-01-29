using Input;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Modules;
using ModulesFrameworkUnity;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using BoardGame.Meta;
using UnityEngine;

namespace EcsCore
{
    [GlobalModule]
    public class GlobalModule : EcsModule
    {
        protected override async Task Setup()
        {
            var tasks = new List<Task>();

            var input = Load<GameObject>("Input", tasks);
            var metaUI = Load<GameObject>("MetaUI", tasks);
            tasks.Add(input);

            var alltask = Task.WhenAll(tasks.ToArray());
            await alltask;

            var inputGO = Object.Instantiate(input.Result);
            var metaUIGO = Object.Instantiate(metaUI.Result);

            world.CreateOneData(new InputData { PlayerInput = inputGO.GetComponent<PlayerInput>() });
            world.CreateOneData(new MainMenuData { UI = metaUIGO });
        }

        private Task<T> Load<T>(string name, List<Task> tasks)
        {
            var task = Addressables.LoadAssetAsync<T>(name).Task;
            tasks.Add(task);
            return task;
        }
    }
}