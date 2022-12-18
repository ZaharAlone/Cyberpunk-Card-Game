using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace BoardGame
{
    [CreateAssetMenu(fileName = "BoardGameRuleSettings", menuName = "Scriptable Object/Board Game/Board Game Rule Settings")]
    public class BoardGameRuleSettings : SerializedScriptableObject
    {
        [Header ("������� ��������� ������")]
        public List<Cards> BasePoolCard = new List<Cards>();
        public int BaseInfluenceCount = 50;

        [Header("��������� ����")]
        [Tooltip("���-�� �������� ���� � ��������")]
        public int OpenCardInShop = 5;

        [Tooltip("����� ����� ����������� � �������� ����� ����� ������")]
        public string KeyCardsInShopAdd;

        [Tooltip("������� ���� ������ ����� �������� � ���� � ������ ���")]
        public int CardInHandFirstPlayerOneRound = 3;
        [Tooltip("������� ���� ������ ����� �������� � ���� � ���")]
        public int BaseCountDropCard = 5;
    }
    
    [System.Serializable]
    public struct Cards
    {
        public string Key;
        public int Value;
    }
}