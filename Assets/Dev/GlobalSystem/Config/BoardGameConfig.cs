using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using CyberNet.Core;

namespace CyberNet
{
    [CreateAssetMenu(fileName = "BoardGameConfig", menuName = "Scriptable Object/Board Game/Board Game Config")]
    public class BoardGameConfig : SerializedScriptableObject
    {
        [Header("Prefab")]
        public GameObject TablePrefab;
        public CardMono CardMono;

        [Header("Element ability card")]
        public Image IconsBaseAbility;
        public Image IconsArrowBaseAbility;
        public Image IconsArrowConditionAbility;
        public TextMeshProUGUI TextBaseAbility;
        public GameObject ItemIconsCounterCard;

        [Header("Config Card")]
        public TextAsset CardConfigJson;

        [Header("Dictionary Links")]
        public Dictionary<string, Sprite> NationsImage;
        public Dictionary<string, Sprite> CurrencyImage;
        public Dictionary<string, Sprite> CardImage;

        public float StepPosXPlayerDown = 210f;
        public float StepPosXPlayerUp = 170f;

        [Header("Positions Player")]
        public Vector2 PlayerHandPosition = new Vector2(0, -330);

        public Vector2 PlayerCardPositionInPlay = new Vector2(0, -120);

        [Header("Positions Enemy")]
        public Vector2 EnemyHandPosition = new Vector2(0, 330);

        [Header("Size")]
        public Vector3 SizeCardInDeck = new Vector3(0.25f, 0.25f, 1f);
        public Vector3 SizeCardInTable = new Vector3(0.7f, 0.7f, 1f);
        public Vector3 SizeCardPlayerUp = new Vector3(0.5f, 0.5f, 1f);
        public Vector3 SizeCardPlayerDown = new Vector3(0.8f, 0.8f, 1f);
        public Vector3 NormalSize = Vector3.one;
        public Vector3 SizeSelectCardHand = new Vector3(1.4f, 1.4f, 1.4f);
        public Vector3 SizeSelectCardTradeRow = new Vector3(1.8f, 1.8f, 1.8f);
    }
}