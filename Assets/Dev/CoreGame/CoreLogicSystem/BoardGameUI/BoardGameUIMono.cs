using CyberNet.Core.AbilityCard.DestroyCard;
using UnityEngine;
using CyberNet.Core.AbilityCard.UI;
using CyberNet.Core.Arena.ArenaHUDUI;
using CyberNet.Core.BezierCurveNavigation;
using CyberNet.Core.Dialog;
using CyberNet.Core.WinLose;
using CyberNet.Core.Traderow;
using CyberNet.Core.UI.CorePopup;
using CyberNet.Core.UI.PopupDistrictInfo;
using CyberNet.Core.UI.TaskPlayerPopup;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace CyberNet.Core.UI
{
    public class BoardGameUIMono : MonoBehaviour
    {
        [Required]
        public RectTransform UIRect;
        [Header("Core Game UI HUD")]
        public CoreHUDUIMono CoreHudUIMono;

        [Header("View Change Round")]
        public ChangeRoundUIMono ChangeRoundUI;
        [Header("Select Ability Mono")]
        public SelectAbilityUIMono SelectAbilityUIMono;
        [Header("Task Player Popup UI Mono")]
        public TaskPlayerPopupUIMono TaskPlayerPopupUIMono;

        [Header("Visual Effect")]
        public CharacterDamageScreenEffectMono DamageScreen;

        [Header("Dialog")]
        public DialogUIMono DialogUIMono;

        [Header("Win Lose Screen")]
        public WinLoseUIMono WinLoseUIMono;

        [Header("Cards Container")]
        public Transform CardsContainer;

        [Header("Traderow")]
        public TraderowMono TraderowMono;
        
        [Header("Popup Card")]
        public CoreElementInfoPopupUIMono GameElementInfoPopupMono;

        [Header("Ability Cancel Button UI")]
        public AbilityInputButtonUIMono AbilityInputButtonUIMono;

        public BezierCurveUIMono BezierCurveUIMono;

        [Header("Arena")]
        public ArenaHUDUIMono ArenaHUDUIMono;

        [Header("Popup District")]
        public PopupDistrictInfoUIMono PopupDistrictInfoUIMono;
        
        [Header ("Destroy Card")]
        public DestroyCardUIMono DestroyCardUIMono;

        public GameObject BlockRaycastPanel;
    }
}