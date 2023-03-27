using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using BoardGame.Core;

namespace BoardGame
{
    [CreateAssetMenu(fileName = "BoardGameConfig", menuName = "Scriptable Object/Board Game/Board Game Config")]
    public class BoardGameConfig : SerializedScriptableObject
    {
        [Header("Prefab")]
        public GameObject TablePrefab;
        public CardMono CardMono;

        [Header("Element ability card")]
        public Image IconsBaseAbility;
        public Image IconsMiniAbility;
        public TextMeshProUGUI TextBaseAbility;
        public TextMeshProUGUI TextDotAbility;

        [Header("Config Card")]
        public TextAsset CardConfigJson;

        [Header("Dictionary Links")]
        public Dictionary<string, Sprite> NationsImage;
        public Dictionary<string, Sprite> CurrencyImage;
        public Dictionary<string, Sprite> CardImage;

        [Header("View Card")]
        public GameObject ViewCardCanvas;

        public float StepPosXPlayerDown = 210f;
        public float StepPosXPlayerUp = 170f;

        [Header("Positions Player")]
        public Vector2 PositionsCardDeckPlayer = new Vector2(830, 400);
        public Vector2 PlayerHandPosition = new Vector2(0, -330);

        public Vector2 PlayerCardPositionInPlay = new Vector2(0, -120);

        [Header("Positions Enemy")]
        public Vector2 PositionsCardDeckEnemy = new Vector2(830, -100);
        public Vector2 EnemyHandPosition = new Vector2(0, 330);
        public Vector3 SizeCardPlayerUp = new Vector3(0.5f, 0.5f, 1f);

        [Header("Positions Trade")]
        public Vector2 PositionsShopFirstCard = new Vector2(-700, 0);
        public Vector2 PositionsShopNeutralCard = new Vector2(-1050, 0);

        [Header("Size")]
        public Vector3 SizeCardInDeck = new Vector3(0.8f, 0.8f, 1f);
        public Vector3 SizeCardPlayerDown = new Vector3(0.8f, 0.8f, 1f);
        public Vector3 NormalSize = Vector3.one;

        [Header("Rotate")]
        public Quaternion RotateInTable;
    }
}