using UnityEngine;
using System.Collections.Generic;
using CyberNet.Core.City;
using CyberNet.Global.Config;
using UnityEngine.Serialization;

namespace CyberNet
{
    /// <summary>
    /// Все данные для карточной игра хранятся тут
    /// </summary>
    public struct BoardGameData
    {
        public BoardGameConfig BoardGameConfig;
        public BoardGameRuleSettings BoardGameRule;
        public Dictionary<string, Sprite> CardsImage;
        public CitySO CitySO;
        public Dictionary<string, DistrictConfig> DistrictConfig;
        public SupportLocalizeSO SupportLocalize;
    }
}