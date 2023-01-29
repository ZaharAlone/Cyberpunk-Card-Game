using UnityEngine;
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

        [Header("Config Card")]
        public TextAsset CardConfigJson;

        [Header("Dictionary Links")]
        public Dictionary<string, Sprite> NationsImage;
        public Dictionary<string, Sprite> CurrencyImage;
        public Dictionary<string, Sprite> CardImage;

        [Header("View Card")]
        public GameObject ViewCardCanvas;

        [Header("Positions")]
        public Vector2 PositionsCardDeckPlayerOne = new Vector2(830, 400);
        public Vector2 PositionsCardDeckPlayerTwo = new Vector2(830, -100);

        public Vector2 PositionsShopFirstCard = new Vector2(-700, 0);
        public Vector2 PositionsShopNeutralCard = new Vector2(-1050, 0);

        public Vector2 PlayerHandPosition = new Vector2(0, -330);
        public Vector2 PlayerCardPositionInPlay = new Vector2(0, -120);
        public Vector2 PlayerCardDiscardPosition;

        [Header("Size")]
        public Vector3 SizeCardInDeckAndDrop = new Vector3(0.8f, 0.8f, 1f);
        public Vector3 NormalSize = Vector3.one;
    }
}