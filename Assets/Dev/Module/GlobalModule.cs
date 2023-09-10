using Input;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Modules;
using ModulesFrameworkUnity;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using CyberNet.Meta;
using UnityEngine;
using CyberNet.Core;
using CyberNet;
using CyberNet.Core.ActionCard;
using CyberNet.Core.Dialog;
using CyberNet.Core.Enemy;
using CyberNet.Core.Sound;
using CyberNet.Core.City;
using CyberNet.Global.GameCamera;

namespace EcsCore
{
    [GlobalModule]
    public class GlobalModule : EcsModule
    {
        private List<Object> _resource = new List<Object>();

        protected override async Task Setup()
        {
            var tasks = new List<Task>();

            var camera = Load<GameObject>("GameCamera", tasks);
            var input = Load<GameObject>("Input", tasks);
            var metaUI = Load<GameObject>("MetaUI", tasks);
            var popupUI = Load<GameObject>("PopupCanvas", tasks);
            var boardGameConfig = Load<BoardGameConfig>("BoardGameConfig", tasks);
            var boardGameRule = Load<BoardGameRuleSettings>("BoardGameRuleSettings", tasks);
            var cityVisualSO = Load<CityVisualSO>("CityVisualSO", tasks);
            var cardsImage = Load<CardsImageDictionary>("CardsImage", tasks);
            var leadersView = Load<LeadersViewSO>("LeadersView", tasks);
            var soundList = Load<SoundList>("SoundList", tasks);
            var actionCardEffect = Load<ActionCardConfig>("ActionCardEffect", tasks);
            var dialogConfig = Load<DialogConfigSO>("DialogConfigSO", tasks);
            var botConfig = Load<BotConfigSO>("BotConfig", tasks);
            var popupViewConfig = Load<PopupViewConfigSO>("PopupViewConfig", tasks);
            tasks.Add(input);

            var alltask = Task.WhenAll(tasks.ToArray());
            await alltask;

            var cameraObject = Object.Instantiate(camera.Result);
            var inputGO = Object.Instantiate(input.Result);
            var metaUIGO = Object.Instantiate(metaUI.Result);
            var popupUIGO = Object.Instantiate(popupUI.Result);
            Object.DontDestroyOnLoad(popupUIGO);

            world.CreateOneData(new GameCameraData { CameraGO = cameraObject });
            world.CreateOneData(new InputData { PlayerInput = inputGO.GetComponent<PlayerInput>() });
            world.CreateOneData(new MetaUIData { UIGO = metaUIGO, MetaUIMono = metaUIGO.GetComponent<MetaUIMono>()});
            world.CreateOneData(new PopupData { PopupUIMono = popupUIGO.GetComponent<PopupUIMono>(), PopupViewConfig = popupViewConfig.Result});
            world.CreateOneData(new BoardGameData { BoardGameConfig = boardGameConfig.Result, BoardGameRule = boardGameRule.Result, CardsImage = cardsImage.Result.Cards, CityVisualSO = cityVisualSO.Result});
            world.CreateOneData(new LeadersViewData { LeadersView = leadersView.Result.Avatar });
            world.CreateOneData(new SoundData { Sound = soundList.Result });
            world.CreateOneData(new ActionCardConfigData {ActionCardConfig = actionCardEffect.Result});
            world.CreateOneData(new DialogConfigData { DialogConfigSO = dialogConfig.Result});
            world.CreateOneData(new BotConfigData { BotConfigSO = botConfig.Result});
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