using UnityEngine;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.AbilityCard.UI;
using CyberNet.Core.Dialog;
using CyberNet.Core.SelectFirstBase;
using CyberNet.Core.WinLose;
using CyberNet.Core.TaskUI;
using CyberNet.Core.Traderow;
using CyberNet.Core.UI.CorePopup;
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
        [Header("Ability Action UI Mono")]
        public AbilitySelectElementUIMono AbilitySelectElementUIMono;

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

        [Header("TaskUI")]
        public TaskUIMono TaskUIMono;

        [Header("Select First Base")]
        public SelectFirstBaseUIMono SelectFirstBaseUIMono;

        [Header("Popup Card")]
        public CoreElementInfoPopupUIMono GameElementInfoPopupMono;

        [Header("Ability Cancel Button UI")]
        public AbilityCancelButtonUIMono AbilityCancelButtonUIMono;
    }
}