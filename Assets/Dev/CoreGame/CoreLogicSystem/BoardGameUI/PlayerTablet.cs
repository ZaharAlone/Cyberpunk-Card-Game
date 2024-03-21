using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CyberNet.Core.UI
{
    [Serializable]
    public struct PlayerTablet
    {
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI UnitCountText;
        public Image IconsUnit;
        public GameObject VFXEffect_current_turnPlayer;
        public Image Avatar;
    }
}