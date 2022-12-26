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

namespace EcsCore
{
    public class BoardGameModule : EcsModule
    {
        private List<Object> _resource = new List<Object>();

        protected override async Task Setup()
        {
            var tasks = new List<Task>();

            var camera = Load<GameObject>("BoardGameCamera", tasks);
            var ui = Load<GameObject>("BoardGameUI", tasks);
            var boardGameConfig = Load<BoardGameConfig>("BoardGameConfig", tasks);
            var boardGameRule = Load<BoardGameRuleSettings>("BoardGameRuleSettings", tasks);

            var alltask = Task.WhenAll(tasks.ToArray());
            await alltask;

            var cameraObject = Object.Instantiate(camera.Result);
            var uiObject = Object.Instantiate(ui.Result);
            world.CreateOneData(new BoardGameCameraComponent { Camera = cameraObject });
            world.CreateOneData(new BoardGameUIComponent { UIGO = uiObject });
            world.CreateOneData(new BoardGameData { BoardGameConfig = boardGameConfig.Result, BoardGameRule = boardGameRule.Result });

            _resource.Add(cameraObject);
            _resource.Add(uiObject);
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