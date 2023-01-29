using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using I2.Loc;

namespace BoardGame
{
    [CreateAssetMenu(fileName = "BoardGameRuleSettings", menuName = "Scriptable Object/Board Game/Board Game Rule Settings")]
    public class BoardGameRuleSettings : SerializedScriptableObject
    {
        [Header ("Ѕазовые параметры игрока")]
        public List<Cards> BasePoolCard = new List<Cards>();
        public int BaseInfluenceCount = 50;
        public int BaseCyberpsychosisCount = 0;

        [Header("ѕараметры игры")]
        [Tooltip(" ол-во открытых карт в магазине")]
        public int OpenCardInShop = 5;

        [Tooltip(" акие карты присутсвуют в магазине кроме общей колоды")]
        public string KeyCardsInShopAdd;

        [Tooltip("—колько карт первый игрок получает в руку в первый ход")]
        public int CardInHandFirstPlayerOneRound = 3;
        [Tooltip("—колько карт обычно игрок получает в руку в ход")]
        public int BaseCountDropCard = 5;

        public LocalizedString ActionPlayAll;
        public LocalizedString ActionAttack;
        public LocalizedString ActionEndTurn;
    }
    
    [System.Serializable]
    public struct Cards
    {
        public string Key;
        public int Value;
    }
}