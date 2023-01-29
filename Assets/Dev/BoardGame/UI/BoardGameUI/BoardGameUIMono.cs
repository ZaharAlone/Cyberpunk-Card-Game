using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BoardGame.Core.UI
{
    public class BoardGameUIMono : MonoBehaviour
    {
        public RectTransform UIRect;

        public TextMeshProUGUI ValueAttackText;
        public TextMeshProUGUI ValueTradeText;
        public Image InteractiveZoneImage;

        [Header("Stats Players")]
        public TextMeshProUGUI PlayerStats;
        public TextMeshProUGUI EnemyStats;

        [Header("Action Button")]
        public GameObject ActionButton;
        public TextMeshProUGUI ActionButtonText;

        public void SetInteractiveValue(int attackValue, int tradeValue)
        {
            ValueAttackText.text = attackValue.ToString();
            ValueTradeText.text = tradeValue.ToString();
        }

        public void SetPlayerStats(int influenceValue)
        {
            PlayerStats.text = influenceValue.ToString();
        }

        public void SetEnemyStats(int influenceValue)
        {
            EnemyStats.text = influenceValue.ToString();
        }

        public void SetInteractiveButton(string text)
        {
            ActionButtonText.text = text;
        }

        public void ShowInteractiveButton()
        {
            ActionButton.SetActive(true);
        }

        public void HideInteractiveButton()
        {
            ActionButton.SetActive(false);
        }

        public void OnClickActionButton()
        {
            ActionButtonEvent.ClickActionButton?.Invoke();
        }
    }
}