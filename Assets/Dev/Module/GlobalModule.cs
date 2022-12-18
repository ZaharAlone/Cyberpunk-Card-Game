using Input;
using ModulesFramework.Attributes;
using ModulesFramework.Modules;
using ModulesFrameworkUnity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;

namespace EcsCore
{
    [GlobalModule]
    public class GlobalModule : EcsModule
    {
        private Dictionary<Type, object> _dependencies = new Dictionary<Type, object>();

        protected override async Task Setup()
        {
            var inputData = new InputData();

            var tasks = new List<Task>();

            var input = Addressables.InstantiateAsync("Input").Task;
            tasks.Add(input);

            var alltask = Task.WhenAll(tasks.ToArray());
            await alltask;

            inputData.PlayerInput = input.Result.GetComponent<PlayerInput>();
            _dependencies[inputData.GetType()] = inputData;
            EcsWorldContainer.World.InitModule<BoardGameModule>();
        }

        private Task<T> Load<T>(string name, List<Task> tasks)
        {
            var task = Addressables.LoadAssetAsync<T>(name).Task;
            tasks.Add(task);
            return task;
        }

        public override Dictionary<Type, object> GetDependencies()
        {
            return _dependencies;
        }
    }
}