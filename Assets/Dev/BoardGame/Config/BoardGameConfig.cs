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

        [Header("Positions")]
        public Vector2 PositionsCardDeskPlayerOne = new Vector2(-1050, 500);
        public Vector2 PositionsCardDeskPlayerTwo = new Vector2(1050, -500);

        public Vector2 PositionsShopDeckCard = new Vector2(-700, 0);
        public Vector2 PositionsShopFirstCard = new Vector2(-700, 0);
        public Vector2 PositionsShopNeutralCard = new Vector2(-1050, 0);

        public Vector2 PlayerHandPosition = new Vector2(0, -330);

        [Header("Size")]
        public float SizeCardInPlayer = 0.5f;
        public float SizeCardInPlayerTable = 0.5f;
        public float SizeCardInNeutralShop = 0.8f;
    }
}