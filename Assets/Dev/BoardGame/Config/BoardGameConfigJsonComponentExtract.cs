using EcsCore;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFrameworkUnity;

namespace BoardGame
{
    /// <summary>
    /// Читаем Json с конфигом кард и записываем его в компонент
    /// </summary>
    [EcsSystem(typeof(BoardGameModule))]
    public class BoardGameConfigJsonComponentExtract : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            var boardGameData = _dataWorld.GetOneData<BoardGameData>().GetData();
            var dictionary = JsonConvert.DeserializeObject<List<CardStats>>(boardGameData.BoardGameConfig.CardConfigJson.text);
            _dataWorld.CreateOneData(new BoardGameConfigJson { CardConfig = dictionary });
        }
    }
}