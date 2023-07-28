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
using CyberNet.Core.Ability;
using CyberNet.Core.Dialog;
using CyberNet.Core.Sound;

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
            var metaUI = Load<GameObject>("MetaUI", tasks);
            var popupUI = Load<GameObject>("PopupCanvas", tasks);
            var boardGameConfig = Load<BoardGameConfig>("BoardGameConfig", tasks);
            var boardGameRule = Load<BoardGameRuleSettings>("BoardGameRuleSettings", tasks);
            var cardsImage = Load<CardsImageDictionary>("CardsImage", tasks);
            var leadersView = Load<LeadersViewSO>("LeadersView", tasks);
            var soundList = Load<SoundList>("SoundList", tasks);
            var cardAbilitEffect = Load<CardAbilityEffect>("CardAbilityEffect", tasks);
            var dialogConfig = Load<DialogConfigSO>("DialogConfigSO", tasks);
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
            world.CreateOneData(new MetaUIData { UIGO = metaUIGO, MetaUIMono = metaUIGO.GetComponent<MetaUIMono>()});
            world.CreateOneData(new PopupData { UIMono = popupUIGO.GetComponent<PopupUIMono>() });
            world.CreateOneData(new BoardGameData { BoardGameConfig = boardGameConfig.Result, BoardGameRule = boardGameRule.Result, CardsImage = cardsImage.Result.Cards});
            world.CreateOneData(new LeadersViewData { LeadersView = leadersView.Result.Avatar });
            world.CreateOneData(new SoundData { Sound = soundList.Result });
            world.CreateOneData(new CardAbilityEffectData {CardAbilityEffect = cardAbilitEffect.Result});
            world.CreateOneData(new DialogConfigData { DialogConfigSO = dialogConfig.Result});
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