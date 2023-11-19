using System;
using System.Collections.Generic;
using CyberNet.Core.EnemyPassport;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace  CyberNet.Core.UI
{
    public class CoreHUDUIMono : MonoBehaviour
    {
        [Header("Stats Players")]
        public PlayerTablet PlayerDownView;

        [Header("Action Button")]
        public GameObject ActionButton;
        public TextMeshProUGUI ActionButtonText;
        public Image ActionButtonImage;

        [Header("Draw and Discard")]
        public RectTransform DownDiscard;
        public TextMeshProUGUI DownDiscardCount;
        public RectTransform DownDeck;
        public TextMeshProUGUI DownDeckCount;
        public RectTransform PositionForUseCardPlayer;

        [Header("Enemy Passport")]
        public List<EnemyPassportFrameUIMono> EnemyPassports = new();

        public void SetMainViewPassportNameAvatar(string name, Sprite avatar)
        {
            PlayerDownView.NameText.text = name;
            PlayerDownView.Avatar.sprite = avatar;
        }

        public void SetMainPassportViewStats(int unit, int victoryPoint, int countAgent)
        {
            PlayerDownView.UnitCountText.text = unit.ToString();
            PlayerDownView.VictoryPointText.text = victoryPoint.ToString();

            for (int i = 0; i < PlayerDownView.AgentIcons.Count; i++)
            {
                if (i < countAgent)
                {
                    PlayerDownView.AgentIcons[i].SetActive(true);
                }
                else
                {
                    PlayerDownView.AgentIcons[i].SetActive(false);
                }
            }
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

        public void SetCountCard(int discardCount, int deckCount)
        {
            DownDiscardCount.text = discardCount.ToString();
            DownDeckCount.text = deckCount.ToString();
        }
        
        public void OnClickOpenDrawDeckCard()
        {
            ShowViewDeckCardAction.OpenDraw?.Invoke();
        }

        public void OnClickOpenDiscardDeckCard()
        {
            ShowViewDeckCardAction.OpenDiscard?.Invoke();
        }

        public void OnSelectPlayer()
        {
            foreach (var enemyPassport in EnemyPassports)
            {
                enemyPassport.OnEffectSelectPlayerStatus();
            }
        }
        
        public void OffSelectPlayer()
        {
            foreach (var enemyPassport in EnemyPassports)
            {
                enemyPassport.OffEffectSelectPlayerStatus();
            }
        }
    }
    
    [Serializable]
    public struct PlayerTablet
    {
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI UnitCountText;
        public TextMeshProUGUI VictoryPointText;
        public Image Avatar;
        public List<GameObject> AgentIcons;
    }
}
