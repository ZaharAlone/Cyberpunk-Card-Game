using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using CyberNet.Core.Ability;
using CyberNet.Core.Dialog;
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
        public AbilityActionUIMono AbilityActionUIMono;

        [Header("Visual Effect")]
        public CharacterDamageScreenEffect DamageScreen;

        [Header("Dialog")]
        public DialogUIMono DialogUIMono;
    }
}