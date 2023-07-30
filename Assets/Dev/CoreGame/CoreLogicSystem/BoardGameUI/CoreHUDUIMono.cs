using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace  CyberNet.Core.UI
{
    public class CoreHUDUIMono : MonoBehaviour
    {
        [Header("Action Table")]
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

        [Header("Draw and Discard")]
        public Transform DownDiscard;
        public TextMeshProUGUI DownDiscardCount;
        public Transform DownDeck;
        public TextMeshProUGUI DownDeckCount;
        public Transform UpDiscard;
        public TextMeshProUGUI UpDiscardCount;
        public Transform UpDeck;
        public TextMeshProUGUI UpDeckCount;

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
            ActionPlayerButtonEvent.ClickActionButton?.Invoke();
        }

        public void SetCountCard(int downDiscard, int downDeck,int upDiscard, int upDeck)
        {
            DownDiscardCount.text = downDiscard.ToString();
            DownDeckCount.text = downDeck.ToString();
            UpDiscardCount.text = upDiscard.ToString();
            UpDeckCount.text = upDeck.ToString();
        }
        
        public void OnClickOpenDrawDeckCard()
        {
            ShowViewDeckCardAction.OpenDraw?.Invoke();
        }

        public void OnClickOpenDiscardDeckCard()
        {
            ShowViewDeckCardAction.OpenDiscard?.Invoke();
        }
    }
    
    [Serializable]
    public struct PlayerTablet
    {
        public CharacterDamagePassportEffect CharacterDamagePassportEffect;
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI HPText;
        public Image CyberpsychosisImage;
        public Image Avatar;

        public Transform FrameEffectCard;
    }
}
