using Input;
using ModulesFramework.Attributes;
using ModulesFramework.Modules;
using ModulesFrameworkUnity;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using CyberNet.Meta;
using UnityEngine;
using CyberNet;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.Dialog;
using CyberNet.Core.AI;
using CyberNet.Core.Battle.TacticsMode;
using CyberNet.Core.BezierCurveNavigation;
using CyberNet.Global.Sound;
using CyberNet.Core.Map;
using CyberNet.Global.BlackScreen;
using CyberNet.Global.Config;
using CyberNet.Global.GameCamera;
using CyberNet.Global.Settings;
using ModulesFramework;
using AbilityCardConfig = CyberNet.Core.AbilityCard.AbilityCardConfig;

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
            var settingsUI = Load<GameObject>("SettingsUI", tasks);
            var popupUI = Load<GameObject>("PopupCanvas", tasks);
            var boardGameConfig = Load<BoardGameConfig>("BoardGameConfig", tasks);
            var boardGameRule = Load<BoardGameRuleSettings>("BoardGameRuleSettings", tasks);
            var cityVisualSO = Load<CitySO>("CityVisualSO", tasks);
            var cardsViewConfig = Load<CardsViewConfigSO>("CardsViewConfigSO", tasks);
            var leadersView = Load<LeadersViewSO>("LeadersView", tasks);
            var soundList = Load<SoundList>("SoundList", tasks);
            var abilityCardEffect = Load<AbilityCardConfig>("AbilityCardEffect", tasks);
            var dialogConfig = Load<DialogConfigSO>("DialogConfigSO", tasks);
            var botConfig = Load<BotConfigSO>("BotConfig", tasks);
            var popupViewConfig = Load<PopupViewConfigSO>("PopupViewConfig", tasks);
            var supportLocalize = Load<SupportLocalizeSO>("SupportLocalizeSO", tasks);
            var bezierCurveConfigSO = Load<BezierCurveConfigSO>("BezierCurveConfigSO", tasks);
            var battleTacticsSO = Load<BattleTacticsSO>("BattleTacticsConfig", tasks);
            var blackScreenUI = Load<GameObject>("BlackScreenUI", tasks);
            tasks.Add(input);

            var alltask = Task.WhenAll(tasks.ToArray());
            await alltask;

            var cameraObject = Object.Instantiate(camera.Result);
            var inputGO = Object.Instantiate(input.Result);
            var metaUIGO = Object.Instantiate(metaUI.Result);
            var settingsUIGO = Object.Instantiate(settingsUI.Result);
            var popupUIGO = Object.Instantiate(popupUI.Result);
            var blackScreenUIGO = Object.Instantiate(blackScreenUI.Result);
            Object.DontDestroyOnLoad(popupUIGO);

            world.CreateOneData(new GameCameraData { CameraGO = cameraObject });
            world.CreateOneData(new InputData { PlayerInput = inputGO.GetComponent<PlayerInput>() });
            world.CreateOneData(new MetaUIData {
                UIGO = metaUIGO,
                MetaUIMono = metaUIGO.GetComponent<MetaUIMono>(),
                SettingsUIMono = settingsUIGO.GetComponent<SettingsUIMono>()
            });
            
            world.CreateOneData(new PopupData {
                PopupUIMono = popupUIGO.GetComponent<PopupUIMono>(),
                PopupViewConfig = popupViewConfig.Result
            });
            
            world.CreateOneData(new BoardGameData {
                BoardGameConfig = boardGameConfig.Result,
                BoardGameRule = boardGameRule.Result,
                CardsViewConfig = cardsViewConfig.Result, 
                CitySO = cityVisualSO.Result,
                SupportLocalize = supportLocalize.Result
            });
            
            world.CreateOneData(new LeadersViewData {
                LeadersView = leadersView.Result.Avatar,
                NeutralLeaderAvatar = leadersView.Result.NeutralUnitAvatar,
            });

            var battleTacticsSOResult = battleTacticsSO.Result;
            world.CreateOneData(new BattleTacticsData {
                BattleTactics = battleTacticsSOResult.BattleTactics,
                CardFoTacticsScreen = battleTacticsSOResult.CardFoTacticsScreen,
                KeyNeutralBattleCard = battleTacticsSOResult.KeyNeutralBattleCard,
            });
            
            world.CreateOneData(new SoundData { Sound = soundList.Result});
            world.CreateOneData(new AbilityCardConfigData {AbilityCardConfig = abilityCardEffect.Result});
            world.CreateOneData(new DialogConfigData { DialogConfigSO = dialogConfig.Result});
            world.CreateOneData(new BotConfigData { BotConfigSO = botConfig.Result});
            world.CreateOneData(new BezierData { BezierCurveConfigSO = bezierCurveConfigSO.Result});
            world.CreateOneData(new BlackScreenUIData {
                BlackScreenUIMono = blackScreenUIGO.GetComponent<BlackScreenUIMono>()
            });
            
            _resource.Add(cameraObject);

            MF.World.InitModule<MetaModule>(true);
            
            #if STEAM
            MF.World.InitModule<SteamModule>(true);
            #endif
            
            #if UNITY_EDITOR || TEST_BUILD
            MF.World.InitModule<DebugModule>(true);
            #endif
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