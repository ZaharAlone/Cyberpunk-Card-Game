using System;
using System.Collections.Generic;
using CyberNet.Core.EnemyPassport;
using CyberNet.Core.UI.CorePopup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace  CyberNet.Core.UI
{
    public class CoreHUDUIMono : MonoBehaviour
    {
        [Header("Stats Players")]
        public PlayerTablet PlayerDownView;
        [SerializeField]
        private PlayerPassportValueWinProgressUIMono _playerPassportControlTerritoryView;
        [SerializeField]
        private GameObject _playerVfxDownCard;
        
        [Header("Action Button")]
        public GameObject ActionButton;
        public TextMeshProUGUI ActionButtonText;
        public Image ActionButtonImage;
        public CoreElementInfoPopupButtonMono PopupActionButton;

        [Header("Ability Button")]
        public GameObject AbilityButton;
        
        [Header("Draw and Discard")]
        public RectTransform DownDiscard;
        public TextMeshProUGUI DownDiscardCount;
        public RectTransform DownDeck;
        public TextMeshProUGUI DownDeckCount;
        public RectTransform PositionForUseCardPlayer;

        [Header("Enemy Passport")]
        public GameObject EnemyPassportContainer;
        public List<EnemyPassportFrameUIMono> EnemyPassports = new();
        public PlayerEnemyTurnActionUIMono PlayerEnemyTurnActionUIMono;
        
        public void SetMainViewPassportNameAvatar(string name, Sprite avatar, Sprite iconsUnit, Color32 colorUnit)
        {
            PlayerDownView.NameText.text = name;
            PlayerDownView.Avatar.sprite = avatar;
            PlayerDownView.IconsUnit.sprite = iconsUnit;
            PlayerDownView.IconsUnit.color = colorUnit;
        }

        public void EnableMainPlayerCurrentRound(bool status)
        {
            PlayerDownView.VFXEffect_current_turnPlayer.SetActive(status);
            _playerVfxDownCard.SetActive(status);
        }

        public void EnableLeftPlayerCurrentRound(bool status, int playerID)
        {
            foreach (var enemy in EnemyPassports)
            {
                if (enemy.GetPlayerID() == playerID)
                {
                    enemy.EnableCurrentTurnPlayer(status);
                }
            }
        }
        
        public void SetMainPassportViewStats(int unit, int countControlTerritory)
        {
            PlayerDownView.UnitCountText.text = unit.ToString();
            _playerPassportControlTerritoryView.SetCountValue(countControlTerritory);
        }
        
        public void SetInteractiveButton(string text, Sprite sprite)
        {
            ActionButtonText.text = text;
            ActionButtonImage.sprite = sprite;
        }
        
        public void ShowInteractiveButton()
        {
            ActionButton.SetActive(true);
            AbilityButton.SetActive(true);
        }

        public void HideInteractiveButton()
        {
            ActionButton.SetActive(false);
            AbilityButton.SetActive(false);
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

        public void HideEnemyPassport()
        {
            EnemyPassportContainer.SetActive(false);
        }
        
        public void ShowEnemyPassport()
        {
            EnemyPassportContainer.SetActive(true);
        }
    }
}
