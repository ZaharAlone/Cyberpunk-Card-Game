using UnityEngine;
using System.Collections.Generic;

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
    }
}