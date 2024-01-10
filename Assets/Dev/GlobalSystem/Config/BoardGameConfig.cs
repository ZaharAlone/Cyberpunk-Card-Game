using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using CyberNet.Core;
using CyberNet.Core.Arena;
using CyberNet.Core.City;
using CyberNet.Global;
using CyberNet.Global.Config;
using CyberNet.Global.Cursor;
using I2.Loc;
using UnityEngine.Serialization;

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

        [Header("Element ability card")]
        public Image IconsBaseAbility;
        public TextMeshProUGUI TextBaseAbilityCountItem;
        public Image IconsArrowConditionAbility;
        public TextMeshProUGUI TextBaseAbility;
        public GameObject ItemIconsCounterCard;
        public GameObject IconsArrowChooseAbility;

        [Header("Color Resource")]
        public Color32 ColorAttackText;
        public Color32 ColorTradeText;

        [Header("Config Card")]
        public TextAsset CardConfigJson;
        public TextAsset PopupCardConfigJson;
        public TextAsset AbilityCardConfigJson;

        [Header("Leader Config")]
        public TextAsset LeaderConfigJson;
        [FormerlySerializedAs("AbilityConfigJson")]
        public TextAsset AbilityLeaderConfigJson;
        
        [Header("Dictionary Links")]
        public Dictionary<string, Sprite> NationsImage;
        public Dictionary<string, Sprite> CurrencyImage;
        public Dictionary<PlayerTypeEnum, LocalizedString> PlayerTypeLoc;

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
        public ArenaConfigSO ArenaConfigSO;
        public ColorsGameConfigSO ColorsGameConfigSO;
    }
}