using System.Threading.Tasks;
using CyberNet.Core.InteractiveCard;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using I2.Loc;
using DG.Tweening;
using UnityEngine.Serialization;

namespace CyberNet.Core
{
    public class CardMono : MonoBehaviour
    {
        public InteractiveCardMono InteractiveCard;
        public Canvas Canvas;
        public RectTransform RectTransform;
        public GameObject VFXIsInteractiveCard;
        public GameObject VFXFlashOutlineCard;

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
        public RectTransform ImageDownBlockRect;
        public RectTransform AbilityBlockRect;
        public Localize Header;
        public TextMeshProUGUI Cyberpsychosis;
        public Transform AbilityBlock_1_Container;
        public Transform AbilityBlock_2_Container;
        public Transform AbilityBlock_3_Container;
        public Transform AbilityBlock_OneShot_Container;
        public GameObject SelectOneCardHeaderGO;
        public Localize SelectOneCardHeaderText;
        public GameObject DivideLine;

        public Transform CountCardBlock;

        [Header("Localizations")]
        [SerializeField]
        private LocalizedString _chooseOneCardHeaderLoc;
        [SerializeField]
        private LocalizedString _useOneCardHeaderLoc;

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

        public void SetHeaderSelectAbility(bool status, bool isDifferentAbility)
        {
            if (isDifferentAbility)
                SelectOneCardHeaderText.Term = _useOneCardHeaderLoc.mTerm;
            else
                SelectOneCardHeaderText.Term = _chooseOneCardHeaderLoc.mTerm;
            
            SelectOneCardHeaderGO.SetActive(status);
        }

        public void IsConditionAbility(bool status)
        {
            DivideLine.SetActive(status);
        }

        public void SetBigDownBlock()
        {
            ImageDownBlockRect.sizeDelta = new Vector2(ImageDownBlockRect.sizeDelta.x, 120f);
            AbilityBlockRect.anchoredPosition = new Vector2(AbilityBlockRect.anchoredPosition.x, 120f);
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

        public async void EnableVFXFlashOutlineCard()
        {
            VFXFlashOutlineCard.SetActive(true);
            await Task.Delay(150);
            VFXFlashOutlineCard.SetActive(false);
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