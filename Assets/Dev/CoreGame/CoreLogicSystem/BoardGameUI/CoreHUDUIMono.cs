using System;
using System.Collections.Generic;
using CyberNet.Core.EndTurnWarningPopup;
using CyberNet.Core.EnemyPassport;
using CyberNet.Core.UI.ActionButton;
using CyberNet.Core.UI.CorePopup;
using Sirenix.OdinInspector;
using UnityEngine;

namespace  CyberNet.Core.UI
{
    public class CoreHUDUIMono : MonoBehaviour
    {
        [Header("Stats Players")]
        public PlayerTablet PlayerDownView;
        [SerializeField]
        [Required]
        private GameObject _playerVfxDownCard;

        [Header("Draw and Discard")]
        [Required]
        public RectTransform DownDiscard;
        [SerializeField]
        [Required]
        private DeckButtonViewMono _discardDeckView;
        [Required]
        public RectTransform DownDeck;
        [SerializeField]
        [Required]
        private DeckButtonViewMono _drawDeckView;

        [Header("Enemy Passport")]
        [Required]
        public GameObject EnemyPassportContainer;
        [Required]
        public List<EnemyPassportFrameUIMono> EnemyPassports = new();
        [Required]
        public PlayerEnemyTurnActionUIMono PlayerEnemyTurnActionUIMono;

        [Header("End Turn popup")]
        [Required]
        public EndTurnWarningPopupUIMono EndTurnWarningPopupUIMono;

        [Header("Other")]
        [Required]
        public RectTransform ZoneHandCard;
        
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
        
        public void SetMainPassportViewStats(int unit, int victoryPoint, int victoryPointToFinishGame, int countDiscardCard)
        {
            PlayerDownView.UnitCountText.text = unit.ToString();
            var textVictoryPoint = $"{victoryPoint}/{victoryPointToFinishGame}";
            PlayerDownView.VictoryPoint.text = textVictoryPoint;

            if (countDiscardCard == 0)
            {
                PlayerDownView.ImageDiscardCard.SetActive(false);
                PlayerDownView.TextDiscardCard.text = "";
            }
            else
            {
                PlayerDownView.ImageDiscardCard.SetActive(true);
                var countDiscardCardText = countDiscardCard > 1 ? countDiscardCard.ToString() : "";
                PlayerDownView.TextDiscardCard.text = countDiscardCardText;
            }
        }

        public void SetCountCard(int discardCardCount, int drawDeckCount)
        {
            _drawDeckView.SetCountCardInDeck(drawDeckCount);
            _discardDeckView.SetCountCardInDeck(discardCardCount);
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

        public void ShowCurrentPlayer()
        {
            PlayerDownView.PlayerPanel.SetActive(true);
        }
        
        public void HideCurrentPlayer()
        {
            PlayerDownView.PlayerPanel.SetActive(false);
        }

        public void HideEnemyPassport()
        {
            EnemyPassportContainer.SetActive(false);
        }
        
        public void ShowEnemyPassport()
        {
            EnemyPassportContainer.SetActive(true);
        }
        
        public void ShowButtons()
        {
            DownDiscard.gameObject.SetActive(true);
            DownDeck.gameObject.SetActive(true);
        }
        
        public void HideButtons()
        {
            DownDiscard.gameObject.SetActive(false);
            DownDeck.gameObject.SetActive(false);
        }
    }
}
