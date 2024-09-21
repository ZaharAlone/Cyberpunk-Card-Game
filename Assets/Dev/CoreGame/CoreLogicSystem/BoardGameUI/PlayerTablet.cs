using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CyberNet.Core.UI
{
    [Serializable]
    public struct PlayerTablet
    {
        [Required]
        public TextMeshProUGUI NameText;
        [Required]
        public TextMeshProUGUI UnitCountText;
        [Required]
        public Image IconsUnit;
        [Required]
        public GameObject VFXEffect_current_turnPlayer;
        [Required]
        public Image Avatar;
        [Required]
        public TextMeshProUGUI VictoryPoint;
    }
}