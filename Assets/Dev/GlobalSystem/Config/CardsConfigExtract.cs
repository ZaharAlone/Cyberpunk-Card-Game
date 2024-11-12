using System;
using EcsCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using CyberNet.Tools.DebugGame;
using Newtonsoft.Json;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using UnityEngine.Networking;

namespace CyberNet
{
    /// <summary>
    /// Читаем Json с конфигом кард и записываем его в компонент
    /// </summary>
    [EcsSystem(typeof(GlobalModule))]
    public class CardsConfigExtract : IPreInitSystem
    {
        private DataWorld _dataWorld;

        private const string card_config_url = "https://bitlightgames.com/cybernet/cards.json";
        private string _server_card_config;
        
        public void PreInit()
        {
            #if SERVER_CONFIG
            GetServerConfig();
            #else
            ParseConfig(false);
            #endif
        }

        private async void GetServerConfig()
        {
            var webRequest = UnityWebRequest.Get(card_config_url);
            webRequest.SendWebRequest();

            var counterWhile = 100;
            while (!webRequest.isDone || counterWhile > 0)
            {
                counterWhile--;
                await Task.Yield();
            }

            var configIsCorrect = false;
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                _server_card_config = webRequest.downloadHandler.text;
                
                if (!string.IsNullOrEmpty(_server_card_config))
                {
                    configIsCorrect = true;
                }
            }
            
            ParseConfig(configIsCorrect);
        }

        private void ParseConfig(bool isServerConfig)
        {
            var boardGameData = _dataWorld.OneData<BoardGameData>();

            var cardConfigJson = boardGameData.BoardGameConfig.CardConfigJson.text;
            
            if (isServerConfig)
                cardConfigJson = _server_card_config;
            
            var cardConfig = JsonConvert.DeserializeObject<Dictionary<string, CardConfigJson>>(cardConfigJson);
            var abilityCardConfig = JsonConvert.DeserializeObject<Dictionary<string, AbilityCardConfig>>(boardGameData.BoardGameConfig.AbilityCardConfigJson.text);
            
            _dataWorld.CreateOneData(new CardsConfigData 
            {
                Cards = cardConfig,
                AbilityCard = abilityCardConfig,
            });
            
            DebugAction.ReadyConfigCards?.Invoke();
        }
    }
}