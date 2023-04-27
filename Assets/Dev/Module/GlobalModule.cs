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
using BoardGame.Core;
using BoardGame;

namespace EcsCore
{
    [GlobalModule]
    public class GlobalModule : EcsModule
    {
        private List<Object> _resource = new List<Object>();

        protected override async Task Setup()
        {
            var tasks = new List<Task>();

            var camera = Load<GameObject>("BoardGameCamera", tasks);
            var input = Load<GameObject>("Input", tasks);
            var metaUI = Load<GameObject>("MainMenuUI", tasks);
            var popupUI = Load<GameObject>("PopupCanvas", tasks);
            var boardGameConfig = Load<BoardGameConfig>("BoardGameConfig", tasks);
            var boardGameRule = Load<BoardGameRuleSettings>("BoardGameRuleSettings", tasks);
            var avatar = Load<AvatarListSO>("Avatars", tasks);
            tasks.Add(input);

            var alltask = Task.WhenAll(tasks.ToArray());
            await alltask;

            var cameraObject = Object.Instantiate(camera.Result);
            var inputGO = Object.Instantiate(input.Result);
            var metaUIGO = Object.Instantiate(metaUI.Result);
            var popupUIGO = Object.Instantiate(popupUI.Result);
            Object.DontDestroyOnLoad(popupUIGO);

            world.CreateOneData(new BoardGameCameraComponent { Camera = cameraObject });
            world.CreateOneData(new InputData { PlayerInput = inputGO.GetComponent<PlayerInput>() });
            world.CreateOneData(new MainMenuData { UI = metaUIGO });
            world.CreateOneData(new PopupData { UIMono = popupUIGO.GetComponent<PopupUIMono>() });
            world.CreateOneData(new BoardGameData { BoardGameConfig = boardGameConfig.Result, BoardGameRule = boardGameRule.Result });
            world.CreateOneData(new AvatarData { Avatar = avatar.Result.Avatar });
            _resource.Add(cameraObject);

            ModulesUnityAdapter.world.InitModule<MetaModule>(true);
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