using EcsCore;
using System.Collections.Generic;
using Newtonsoft.Json;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;

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
            var cardConfig = JsonConvert.DeserializeObject<Dictionary<string, CardConfigJson>>(boardGameData.BoardGameConfig.CardConfigJson.text);
            var abilityCardConfig = JsonConvert.DeserializeObject<Dictionary<string, AbilityCardConfig>>(boardGameData.BoardGameConfig.AbilityCardConfigJson.text);
            var popupCardConfig = JsonConvert.DeserializeObject<Dictionary<string, CardPopupConfig>>(boardGameData.BoardGameConfig.PopupCardConfigJson.text);
            
            _dataWorld.CreateOneData(new CardsConfig 
            {
                Cards = cardConfig,
                AbilityCard = abilityCardConfig,
                PopupCard = popupCardConfig
            });
        }
    }
}