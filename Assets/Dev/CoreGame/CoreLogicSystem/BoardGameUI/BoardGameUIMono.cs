using UnityEngine;
using CyberNet.Core.AbilityCard.UI;
using CyberNet.Core.Arena.ArenaHUDUI;
using CyberNet.Core.BezierCurveNavigation;
using CyberNet.Core.Dialog;
using CyberNet.Core.SelectFirstBase;
using CyberNet.Core.WinLose;
using CyberNet.Core.Traderow;
using CyberNet.Core.UI.CorePopup;
using CyberNet.Core.UI.TaskPlayerPopup;
using UnityEngine.Serialization;

namespace CyberNet.Core.UI
{
    public class BoardGameUIMono : MonoBehaviour
    {
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
        public CharacterDamageScreenEffect DamageScreen;

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

        [FormerlySerializedAs("abilityInputButtonUIMono")]
        [FormerlySerializedAs("AbilityCancelButtonUIMono")]
        [Header("Ability Cancel Button UI")]
        public AbilityInputButtonUIMono AbilityInputButtonUIMono;

        public BezierCurveUIMono BezierCurveUIMono;

        [Header("Arena")]
        public ArenaHUDUIMono ArenaHUDUIMono;
    }
}