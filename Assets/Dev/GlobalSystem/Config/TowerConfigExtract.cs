using EcsCore;
using System.Collections.Generic;
using Newtonsoft.Json;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;

namespace CyberNet
{
    /// <summary>
    /// Читаем Json с конфигом tower и записываем его в компонент
    /// </summary>
    [EcsSystem(typeof(GlobalModule))]
    public class TowerConfigExtract : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            ref var boardGameData = ref _dataWorld.OneData<BoardGameData>();
            boardGameData.TowerConfig = JsonConvert.DeserializeObject<Dictionary<string, TowerConfig>>(boardGameData.CitySO.TowerConfig.text);
        }
    }
}