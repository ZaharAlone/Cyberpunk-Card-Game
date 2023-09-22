using EcsCore;
using System.Collections.Generic;
using Newtonsoft.Json;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;

namespace CyberNet
{
    /// <summary>
    /// Читаем Json с конфигом героев и абилок и записываем его в компонент
    /// </summary>
    [EcsSystem(typeof(GlobalModule))]
    public class LeadersConfigExtract : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            var boardGameData = _dataWorld.OneData<BoardGameData>();
            var leadersConfig = JsonConvert.DeserializeObject<Dictionary<string, LeadersConfig>>(boardGameData.BoardGameConfig.HeroesConfigJson.text);
            var abilityConfig = JsonConvert.DeserializeObject<Dictionary<string, AbilityConfig>>(boardGameData.BoardGameConfig.AbilityConfigJson.text);
            _dataWorld.CreateOneData(new LeadersConfigData { LeadersConfig = leadersConfig, AbilityConfig = abilityConfig});
        }
    }
}