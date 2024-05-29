using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine.Serialization;

namespace CyberNet
{
    [CreateAssetMenu(fileName = "BoardGameRuleSettings", menuName = "Scriptable Object/Board Game/Board Game Rule Settings")]
    public class BoardGameRuleSettings : SerializedScriptableObject
    {
        [Header ("Базовые параметры игрока")]
        public List<KeyValue> BasePoolCard = new List<KeyValue>();
        [FormerlySerializedAs("StartCountSquad")]
        public int CountSquad = 12;
        public int StartInitCountSquad = 2;

        [Header("Параметры игры")]
        [Tooltip("Кол-во открытых карт в магазине")]
        public int OpenCardInShop = 5;

        [Tooltip("Сколько карт обычно игрок получает в руку в ход")]
        public int CountDropCard = 5;
        
        [Tooltip("Количество нейтральных юнитов на клетке в начале игры")]
        public int CountNeutralUnitInTower = 2;
        
        [Header("Popup Action Button")]
        public string EndRoundPopup;
        public string PlayAllPopup;
    }
}