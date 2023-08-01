using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using CyberNet.Core.ActionCard;
using CyberNet.Core.Dialog;
using CyberNet.Core.WinLose;
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
        [FormerlySerializedAs("AbilitySelectCardUIMono")]
        [FormerlySerializedAs("abilitySelectCardUIMono")]
        [FormerlySerializedAs("AbilityActionUIMono")]
        [Header("Ability Action UI Mono")]
        public ActionSelectCardUIMono actionSelectCardUIMono;

        [Header("Visual Effect")]
        public CharacterDamageScreenEffect DamageScreen;

        [Header("Dialog")]
        public DialogUIMono DialogUIMono;

        [Header("Win Lose Screen")]
        public WinLoseUIMono WinLoseUIMono;
    }
}