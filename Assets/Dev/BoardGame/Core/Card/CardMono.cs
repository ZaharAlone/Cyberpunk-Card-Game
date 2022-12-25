using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using I2.Loc;

namespace BoardGame.Core
{
    public class CardMono : MonoBehaviour
    {
        public InteractiveCard InteractiveCard;
        public RectTransform RectTransform;

        [Header("Card")]
        public RectTransform CardFace;  
        public RectTransform CardBack;

        [Header("Settings")]
        public Image ImageCard;
        public Image ImageNations;
        [Header("Price")]
        public GameObject PriceGO;
        public TextMeshProUGUI PriceText;

        [Header("Bottom")]
        public TextMeshProUGUI Header;
        public CardAbilityView Ability;
        public Image FractionImage;
        public CardAbilityView FractionAbility;
        public CardAbilityView TrashAbility;

        private bool _cardIsBack;

        public void SetViewCard(Sprite imageCard, string header, int price = 0, Sprite imageNations = null)
        {
            Header.text = LocalizationManager.GetTranslation(header);
            ImageCard.sprite = imageCard;

            if (imageNations != null)
                ImageNations.sprite = imageNations;
            else
                ImageNations.gameObject.SetActive(false);

            if (price != 0)
                PriceText.text = price.ToString();
            else
                PriceGO.SetActive(false);
        }

        public void SetAbility(Sprite currency = null, int currency_value = 0, string ability_parameters = "")
        {
            SetViewAbility(Ability, currency, currency_value, ability_parameters);
        }

        public void SetFractionAbiltity(Sprite fractions, Sprite currency = null, int currency_value = 0, string ability_parameters = "")
        {
            FractionImage.sprite = fractions;
            SetViewAbility(FractionAbility, currency, currency_value, ability_parameters);
        }

        public void SetDropAbility(Sprite currency = null, int currency_value = 0, string ability_parameters = "")
        {
            SetViewAbility(TrashAbility, currency, currency_value, ability_parameters);
        }

        private void SetViewAbility(CardAbilityView ability, Sprite currency = null, int currency_value = 0, string ability_parameters = "")
        {
            ability.GO.SetActive(true);
            if (currency != null)
            {
                ability.ImageCurrency.sprite = currency;
                ability.TextCurrency.text = currency_value.ToString();
            }
            else
            {
                ability.ImageCurrency.gameObject.SetActive(false);
            }

            if (ability_parameters != "")
            {
                ability.TextAbility.text = LocalizationManager.GetTranslation(ability_parameters);
            }
            else
            {
                ability.TextAbility.gameObject.SetActive(false);
            }
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
    }
}