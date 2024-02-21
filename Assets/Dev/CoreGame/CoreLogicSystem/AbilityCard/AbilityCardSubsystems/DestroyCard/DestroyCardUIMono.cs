using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CyberNet.Core.AbilityCard.DestroyCard
{
    public class DestroyCardUIMono : MonoBehaviour
    {
        [SerializeField]
        private GameObject _background;
        [SerializeField]
        private GameObject _panel;
        [SerializeField]
        private RectTransform _rectContainer;
        [SerializeField]
        private Transform _containerCards;

        public RectTransform CenterScreen;
        
        private List<CardMono> _cardsHand = new();

        public void OnEnable()
        {
            _panel.SetActive(false);
            _background.SetActive(false);
        }

        public void EnablePanel()
        {
            _panel.SetActive(true);
            _background.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rectContainer);
        }

        public void HidePanel()
        {
            _panel.SetActive(false);
        }
        
        public void DisablePanel()
        {
            _panel.SetActive(false);
            _background.SetActive(false);
        }

        public void ClearCards()
        {
            foreach (Transform child in _containerCards)
            {
                Destroy(child.gameObject);
            }
            
            _cardsHand.Clear();
        }
        
        public CardMono CreateNewCard(CardMono cardView)
        {
            var cardMono = Instantiate(cardView, _containerCards);
            //cardMono.RectTransform.anchoredPosition = _startPosition;
            _cardsHand.Add(cardMono);
            return cardMono;
        }
    }
}