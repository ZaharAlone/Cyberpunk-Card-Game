using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using I2.Loc;

namespace BoardGame
{
    [CreateAssetMenu(fileName = "BoardGameRuleSettings", menuName = "Scriptable Object/Board Game/Board Game Rule Settings")]
    public class BoardGameRuleSettings : SerializedScriptableObject
    {
        [Header ("������� ��������� ������")]
        public List<KeyValue> BasePoolCard = new List<KeyValue>();
        public int BaseInfluenceCount = 50;
        public int BaseCyberpsychosisCount = 0;

        [Header("��������� ����")]
        [Tooltip("���-�� �������� ���� � ��������")]
        public int OpenCardInShop = 5;

        [Tooltip("����� ����� ����������� � �������� ����� ����� ������")]
        public string KeyCardsInShopAdd;

        [Tooltip("������� ���� ������ ����� �������� � ���� � ������ ���")]
        public int CardInHandFirstPlayerOneRound = 3;
        [Tooltip("������� ���� ������ ����� �������� � ���� � ���")]
        public int BaseCountDropCard = 5;

        public LocalizedString ActionPlayAll_loc;
        public LocalizedString ActionAttack_loc;
        public LocalizedString ActionEndTurn_loc;
        public Sprite ActionPlayAll_image;
        public Sprite ActionAttack_image;
        public Sprite ActionEndTurn_image;
    }
}