using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace BoardGame.Core
{
    public class CardMono : MonoBehaviour
    {
        [Header("Card")]
        public RectTransform CardFace;  
        public RectTransform CardBack;

        [Header("Settings")]
        public Image ImageCard;
        public GameObject NationsGO;
        public Image ImageNations;
        public GameObject PriceGO;
        public TextMeshProUGUI PriceText;

        public TextMeshProUGUI Header;

        public void SetViewCard(Sprite imageCard, Sprite imageNations, string header, int price, bool isFree = false, bool isNeutral = false)
        {
            ImageCard.sprite = imageCard;
            ImageNations.sprite = imageNations;

            Header.text = header;
            if (isFree)
                PriceGO.SetActive(false);
            else
            {
                PriceGO.SetActive(true);
                PriceText.text = price.ToString();
            }
        }

        public void CardOnBack()
        {
            CardFace.gameObject.SetActive(false);
            CardBack.gameObject.SetActive(true);
        }

        public void CardOnFace()
        {
            CardFace.gameObject.SetActive(true);
            CardBack.gameObject.SetActive(false);
        }
    }
}