using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using CyberNet.Core;
using CyberNet.Core.Arena;
using CyberNet.Core.Map;
using CyberNet.Global;
using CyberNet.Global.Config;
using CyberNet.Global.Cursor;
using I2.Loc;

namespace CyberNet
{
    [CreateAssetMenu(fileName = "BoardGameConfig", menuName = "Scriptable Object/Board Game/Board Game Config")]
    public class BoardGameConfig : SerializedScriptableObject
    {
        [Header("Prefab")]
        public CityMono CityMono;
        public CardMono CardGO;
        public ArenaMono ArenaMono;
        public GameObject AnalyticsGO;
        public CardMono CardDestroy;
        
        [Header("Config Card")]
        public TextAsset CardConfigJson;
        public TextAsset PopupCardConfigJson;
        public TextAsset PopupTaskConfigJson;
        public TextAsset AbilityCardConfigJson;

        [Header("Leader Config")]
        public TextAsset LeaderConfigJson;
        
        [Header("Dictionary Links")]
        public Dictionary<string, Sprite> CurrencyImage;
        public Dictionary<string, Color32> CurrencyColor;
        public Dictionary<PlayerOrAI, LocalizedString> PlayerTypeLoc;

        [Header("Size")]
        public Vector3 SizeCardInDeck = new Vector3(0.25f, 0.25f, 1f);
        public Vector3 SizeCardInTable = new Vector3(0.7f, 0.7f, 1f);
        public Vector3 SizeCardInTraderow = new Vector3(0.8f, 0.8f, 1f);
        public Vector3 SizeCardPlayingDeck = new Vector3(0.5f, 0.5f, 1f);
        public Vector3 SizeCardPlayerDown = new Vector3(0.8f, 0.8f, 1f);
        public Vector3 NormalSize = Vector3.one;
        public Vector3 SizeSelectCardHand = new Vector3(1.4f, 1.4f, 1.4f);
        public Vector3 SizeSelectCardTradeRow = new Vector3(1.8f, 1.8f, 1.8f);

        [Header("Other config")]
        public CursorConfigSO CursorConfigSO;
        public ColorsGameConfigSO ColorsGameConfigSO;
        public ViewEnemySO ViewEnemySO;
    }
}