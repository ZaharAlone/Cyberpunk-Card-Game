using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using I2.Loc;
using DG.Tweening;

namespace CyberNet.Core
{
    public class CardMono : MonoBehaviour
    {
        public InteractiveCardMono InteractiveCard;
        public Canvas Canvas;
        public GameObject VFXIsInteractiveCard;
        public Transform CardConteinerTransform;

        [Header("Card")]
        public RectTransform CardFace;  
        public RectTransform CardBack;
        public Image BackCardImage;

        [Header("Settings")]
        public Image ImageCard;
        public Image ImageNations;
        [Header("Price")]
        public TextMeshProUGUI PriceText;

        [Header("Down Block")]
        public Localize Header;
        public TextMeshProUGUI Cyberpsychosis;
        public Transform AbilityBlock_1_Container;
        public Transform AbilityBlock_2_Container;
        public GameObject ChooseOneHeader;
        public GameObject DivideLine;

        public Transform CountCardBlock;

        private bool _cardIsBack;
        private Sequence _sequence;

        public void SetViewCard(Sprite imageCard, string header, int cyberpsychosis, int price = 0,  Sprite imageNations = null)
        {
            Header.Term = header;
            ImageCard.sprite = imageCard;
            Cyberpsychosis.text = cyberpsychosis.ToString();

            if (imageNations != null)
                ImageNations.sprite = imageNations;
            else
                ImageNations.gameObject.SetActive(false);

            if (price != 0)
                PriceText.text = price.ToString();
            else
                PriceText.gameObject.SetActive(false);
        }

        public void SetActiveChooseOne(bool status)
        {
            ChooseOneHeader.SetActive(status);
        }

        public void CardOnBack()
        {
            CardFace.gameObject.SetActive(false);
            CardBack.gameObject.SetActive(true);
            _cardIsBack = true;
        }

        public void CardOnFace()
        {
            _cardIsBack = false;
            CardFace.gameObject.SetActive(true);
            CardBack.gameObject.SetActive(false);
        }

        public void SwitchFaceCard()
        {
            CardFace.gameObject.SetActive(_cardIsBack);
            CardBack.gameObject.SetActive(!_cardIsBack);
            _cardIsBack = !_cardIsBack;
        }

        public void SetStatusInteractiveVFX(bool status)
        {
            VFXIsInteractiveCard.SetActive(status);
        }

        public void HideCard()
        {
            Canvas.gameObject.SetActive(false);
        }

        public void HideBackCardColor()
        {
            BackCardImage.color = new Color32(255, 255, 255, 0);
        }

        public void ShowCard()
        {
            Canvas.gameObject.SetActive(true);
        }

        public void SetMovePositionAnimations(Vector3 positions, Vector3 scale)
        {
            _sequence = DOTween.Sequence();
            var distance = Vector3.Distance(transform.position, positions);
            var time = distance / 600;
            if (time > 0.8f)
                time = 0.8f;
            _sequence.Append(transform.DOMove(positions, time))
                     .Join(transform.DOScale(scale, time));
        }

        public void OnDisable()
        {
            if (_sequence != null)
                _sequence.Kill();
        }
    }
}