using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using I2.Loc;

namespace CyberNet
{
    [CreateAssetMenu(fileName = "BoardGameRuleSettings", menuName = "Scriptable Object/Board Game/Board Game Rule Settings")]
    public class BoardGameRuleSettings : SerializedScriptableObject
    {
        [Header ("Базовые параметры игрока")]
        public List<KeyValue> BasePoolCard = new List<KeyValue>();
        public int StartCountUnit = 40;
        public int CountAgentPlayer = 4;
        public int PriceKillUnit = 3;
        public int PricePostAgent = 3;

        [Header("Параметры игры")]
        [Tooltip("Кол-во открытых карт в магазине")]
        public int OpenCardInShop = 5;

        [Tooltip("Сколько карт обычно игрок получает в руку в ход")]
        public int CountDropCard = 5;

        [Header("Localize")]
        public LocalizedString ActionPlayAll_loc;
        public LocalizedString ActionEndTurn_loc;

        [Header("Icons")]
        public Sprite ActionPlayAll_image;
        public Sprite ActionEndTurn_image;
    }
}