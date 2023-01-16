using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector.Editor;

namespace BoardGame.Core.UI
{
    public class BoardGameUIMono : MonoBehaviour
    {
        public TextMeshProUGUI ValueAttackText;
        public TextMeshProUGUI ValueTradeText;
        public Image InteractiveZoneImage;

        [Header("Action Button")]
        public GameObject ActionButton;
        public TextMeshProUGUI ActionButtonText;
    }
}