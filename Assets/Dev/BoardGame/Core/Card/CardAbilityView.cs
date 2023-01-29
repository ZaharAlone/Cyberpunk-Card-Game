using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BoardGame.Core
{
    [System.Serializable]
    public struct CardAbilityView
    {
        public GameObject GO;
        public Image ImageCurrency;
        public TextMeshProUGUI TextCurrency;
        public TextMeshProUGUI TextAbility;
    }
}