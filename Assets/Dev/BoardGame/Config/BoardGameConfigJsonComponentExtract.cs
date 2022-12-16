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
        private BoardGameData _boardGameData;

        public void PreInit()
        {
            var dictionary = JsonConvert.DeserializeObject<List<CardStats>>(_boardGameData.BoardGameConfig.CardConfigJson.text);
            var entity = EcsWorldContainer.World.NewEntity();

            var component = new BoardGameConfigJsonComponent();
            component.CardConfig = dictionary;
            entity.AddComponent(component);
        }
    }
}