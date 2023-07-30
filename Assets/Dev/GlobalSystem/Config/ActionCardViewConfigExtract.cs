using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using System.Collections.Generic;
using CyberNet.Core.ActionCard;
using Newtonsoft.Json;

namespace CyberNet
{
    [EcsSystem(typeof(GlobalModule))]
    public class ActionCardViewConfigExtract : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            ref var actionCardConfig = ref _dataWorld.OneData<ActionCardConfigData>();
            actionCardConfig.ActionCardViewConfig = JsonConvert.DeserializeObject<Dictionary<string, ActionCardViewConfig>>(actionCardConfig.ActionCardConfig.ActionCardViewJsonConfig.text);
        }
    }
}