using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace BoardGame.Core.UI
{
    public class BoardGameUIMono : MonoBehaviour
    {
        public RectTransform UIRect;

        public TextMeshProUGUI ValueAttackText;
        public TextMeshProUGUI ValueTradeText;
        public Image InteractiveZoneImage;

        [Header("Stats Players")]
        public PlayerTablet PlayerDownView;
        public PlayerTablet PlayerUpView;

        [Header("Action Button")]
        public GameObject ActionButton;
        public TextMeshProUGUI ActionButtonText;
        public Image ActionButtonImage;

        public void SetInteractiveValue(int attackValue, int tradeValue)
        {
            ValueAttackText.text = attackValue.ToString();
            ValueTradeText.text = tradeValue.ToString();
        }

        public void SetViewNameAvatarDownTable(string name, Sprite avatar)
        {
            PlayerDownView.NameText.text = name;
            PlayerDownView.Avatar.sprite = avatar;
        }

        public void SetViewDownTableStats(int hp, int cyberpsychosis)
        {
            PlayerDownView.HPText.text = hp.ToString();
            PlayerDownView.CyberpsychosisImage.fillAmount = (float)cyberpsychosis / 15;
        }

        public void SetViewNameAvatarUpTable(string name, Sprite avatar)
        {
            PlayerUpView.NameText.text = name;
            PlayerUpView.Avatar.sprite = avatar;
        }

        public void SetViewUpTableStats(int hp, int cyberpsychosis)
        {
            PlayerUpView.HPText.text = hp.ToString();
            PlayerUpView.CyberpsychosisImage.fillAmount = (float)cyberpsychosis / 15;
        }

        public void SetInteractiveButton(string text, Sprite sprite)
        {
            ActionButtonText.text = text;
            ActionButtonImage.sprite = sprite;
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

    [Serializable]
    public struct PlayerTablet
    {
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI HPText;
        public Image CyberpsychosisImage;
        public Image Avatar;
    }
}