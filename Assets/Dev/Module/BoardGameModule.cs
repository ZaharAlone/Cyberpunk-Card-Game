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

namespace EcsCore
{
    public class BoardGameModule : EcsModule
    {
        private Dictionary<Type, object> _dependencies = new Dictionary<Type, object>();
        private List<Object> _resource = new List<Object>();

        protected override async Task Setup()
        {
            var boardGameData = new BoardGameData();
            var tasks = new List<Task>();

            var camera = Load<GameObject>("BoardGameCamera", tasks);
            var ui = Load<GameObject>("BoardGameUI", tasks);
            var boardGameConfig = Load<BoardGameConfig>("BoardGameConfig", tasks);
            var boardGameRule = Load<BoardGameRuleSettings>("BoardGameRuleSettings", tasks);

            var alltask = Task.WhenAll(tasks.ToArray());
            await alltask;

            boardGameData.BoardGameConfig = boardGameConfig.Result;
            boardGameData.BoardGameRule = boardGameRule.Result;

            var cameraEntity = EcsWorldContainer.World.NewEntity();
            var cameraObject = Object.Instantiate(camera.Result);
            cameraEntity.AddComponent(new BoardGameCameraComponent { Camera = cameraObject });

            var uiEntity = EcsWorldContainer.World.NewEntity();
            var uiObject = Object.Instantiate(ui.Result);
            uiEntity.AddComponent(new BoardGameUIComponent { UIGO = uiObject });

            _resource.Add(cameraObject);
            _resource.Add(uiObject);

            _dependencies[boardGameData.GetType()] = boardGameData;
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

        public override void OnDeactivate()
        {
            foreach (var item in _resource)
                Object.Destroy(item);
        }
    }
}