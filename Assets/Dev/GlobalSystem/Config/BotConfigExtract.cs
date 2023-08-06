using System.Collections.Generic;
using CyberNet.Core.Enemy;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using Newtonsoft.Json;

namespace CyberNet.Core
{
    [EcsSystem(typeof(GlobalModule))]
    public class BotConfigExtract : IInitSystem
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            ref BotConfigData botData = ref _dataWorld.OneData<BotConfigData>();
            botData.BotScoreCard = JsonConvert.DeserializeObject<Dictionary<string, float>>(botData.BotConfigSO.BotAIConfigJson.text);
        }
    }
}