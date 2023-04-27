using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using I2.Loc;

namespace BoardGame
{
    [CreateAssetMenu(fileName = "BoardGameRuleSettings", menuName = "Scriptable Object/Board Game/Board Game Rule Settings")]
    public class BoardGameRuleSettings : SerializedScriptableObject
    {
        [Header ("Базовые параметры игрока")]
        public List<KeyValue> BasePoolCard = new List<KeyValue>();
        public int BaseInfluenceCount = 50;
        public int BaseCyberpsychosisCount = 0;

        [Header("Параметры игры")]
        [Tooltip("Кол-во открытых карт в магазине")]
        public int OpenCardInShop = 5;

        [Tooltip("Какие карты присутсвуют в магазине кроме общей колоды")]
        public string KeyCardsInShopAdd;

        [Tooltip("Сколько карт обычно игрок получает в руку в ход")]
        public int CountDropCard = 5;

        [Header("Localize")]
        public LocalizedString ActionPlayAll_loc;
        public LocalizedString ActionAttack_loc;
        public LocalizedString ActionEndTurn_loc;
        public LocalizedString ChoseOneCard_Loc;

        [Header("Icons")]
        public Sprite ActionPlayAll_image;
        public Sprite ActionAttack_image;
        public Sprite ActionEndTurn_image;
    }
}