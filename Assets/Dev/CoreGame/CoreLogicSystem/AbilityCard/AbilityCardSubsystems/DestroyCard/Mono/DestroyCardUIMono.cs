using System.Collections.Generic;
using I2.Loc;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace CyberNet.Core.AbilityCard.DestroyCard
{
    public class DestroyCardUIMono : MonoBehaviour
    {
        [Header("Global")]
        [SerializeField]
        [Required]
        private GameObject _background;
        [SerializeField]
        [Required]
        private GameObject _panel;

        [Header("Header")]
        [SerializeField]
        [Required]
        private Localize _headerPopup;
        [SerializeField]
        [Required]
        private Localize _descriptionPopup;
        [SerializeField]
        [Required]
        private LocalizationParamsManager _descriptionPopupParams;
        
        [Header("Element")]
        [SerializeField]
        [Required]
        private RectTransform _rectContainer;
        [SerializeField]
        [Required]
        private Transform _containerCards;
        [SerializeField]
        [Required]
        private GameObject _button_complete;

        [Required]
        public Transform ContainerForDestroyCard;
        [Required]
        public RectTransform TransformDestroyZone;
        
        private List<CardMono> _cardsHand = new();
        private List<CardMono> _cardsDiscard = new();
        private List<CardMono> _cardsPlayZone = new();

        public void OnEnable()
        {
            _panel.SetActive(false);
            _background.SetActive(false);
            SetEnableButtonComplete(false);
        }

        public void EnablePanel(string header, string descr, string valueCountParamDescr = "")
        {
            _headerPopup.Term = header;
            _descriptionPopup.Term = descr;
            _descriptionPopupParams.SetParameterValue("count", valueCountParamDescr);
            
            _panel.SetActive(true);
            _background.SetActive(true);
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
            _cardsDiscard.Clear();
            _cardsPlayZone.Clear();
        }

        public void SetEnableButtonComplete(bool value)
        {
            _button_complete.SetActive(value);
        }
        
        public CardMono CreateNewCard(CardMono cardView, GroupDestroyCardEnum targetGroupDestroyCardEnum)
        {
            var cardMono = Instantiate(cardView, _containerCards);

            switch (targetGroupDestroyCardEnum)
            {
                case GroupDestroyCardEnum.PlayZone:
                    _cardsPlayZone.Add(cardMono);
                    break;
                case GroupDestroyCardEnum.Discard:
                    _cardsDiscard.Add(cardMono);
                    break;
                case GroupDestroyCardEnum.Hand:
                    _cardsHand.Add(cardMono);
                    break;
            }            
            
            return cardMono;
        }

        public void SelectCardInHand()
        {
            SwitchViewTargetDiscard(_cardsHand);
        }

        public void SelectCardInPlayZone()
        {
            SwitchViewTargetDiscard(_cardsPlayZone);
        }

        public void SelectCardInDiscard()
        {
            SwitchViewTargetDiscard(_cardsDiscard);
        }

        private void SwitchViewTargetDiscard(List<CardMono> targetList)
        {
            foreach (var cardMono in _cardsHand)
                cardMono.gameObject.SetActive(false);
            
            foreach (var cardMono in _cardsPlayZone)
                cardMono.gameObject.SetActive(false);
            
            foreach (var cardMono in _cardsDiscard)
                cardMono.gameObject.SetActive(false);
            
            foreach (var cardMono in targetList)
                cardMono.gameObject.SetActive(true);
        }

        public void OnClickCancelPlayingAbility()
        {
            DisablePanel();
            DestroyCardAction.ForceCancelDestroyCard?.Invoke();
        }

        public void OnClickCompletePlayingAbility()
        {
            DisablePanel();
            DestroyCardAction.ForceCompleteDestroyCard?.Invoke();
        }
    }
}