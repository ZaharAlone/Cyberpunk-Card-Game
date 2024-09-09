using CyberNet.Core.InteractiveCard;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace CyberNet.Core
{
    public class CardMono : MonoBehaviour
    {
        public InteractiveCardMono InteractiveCard;
        public Canvas Canvas;
        public RectTransform RectTransform;

        [Header("Card")]
        public RectTransform CardFace;
        public CardFaceMono CardFaceMono;
        public RectTransform CardBack;
        public Image BackCardImage;

        private bool _cardIsBack;
        private Sequence _sequence;

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

        public void OnDisable()
        {
            if (_sequence != null)
                _sequence.Kill();
        }
    }
}