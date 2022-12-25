using Input;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Modules;
using ModulesFrameworkUnity;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;

namespace EcsCore
{
    [GlobalModule]
    public class GlobalModule : EcsModule
    {
        protected override async Task Setup()
        {
            var tasks = new List<Task>();

            var input = Addressables.InstantiateAsync("Input").Task;
            tasks.Add(input);

            var alltask = Task.WhenAll(tasks.ToArray());
            await alltask;

            world.CreateOneData(new InputData { PlayerInput = input.Result.GetComponent<PlayerInput>() });
            EcsWorldContainer.World.InitModule<BoardGameModule>(true);
        }

        private Task<T> Load<T>(string name, List<Task> tasks)
        {
            var task = Addressables.LoadAssetAsync<T>(name).Task;
            tasks.Add(task);
            return task;
        }
    }
}