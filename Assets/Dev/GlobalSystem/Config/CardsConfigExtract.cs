using EcsCore;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFrameworkUnity;

namespace CyberNet
{
    /// <summary>
    /// Читаем Json с конфигом кард и записываем его в компонент
    /// </summary>
    [EcsSystem(typeof(GlobalModule))]
    public class CardsConfigExtract : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            var boardGameData = _dataWorld.OneData<BoardGameData>();
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, CardConfig>>(boardGameData.BoardGameConfig.CardConfigJson.text);
            _dataWorld.CreateOneData(new CardsConfig { Cards = dictionary });
        }
    }
}