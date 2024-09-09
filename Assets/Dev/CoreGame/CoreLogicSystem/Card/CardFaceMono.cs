using System.Threading.Tasks;
using I2.Loc;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
namespace CyberNet.Core
{
    public class CardFaceMono : MonoBehaviour
    {
        [Header("Settings")]
        public Image ImageCard;
        public Image ImageNations;

        [Header("Up Bar")]
        [Required]
        public Localize Header;
        public TextMeshProUGUI PriceText;

        [Header("Down Block")]
        [Required]
        public RectTransform ImageDownBlockRect;
        [Required]
        public RectTransform AbilityBlockRect;
        [Required]
        public TextMeshProUGUI Cyberpsychosis;
        [FormerlySerializedAs("AbilityBlock_1_Container")]
        [Required]
        public Transform AbilityBlock_0_Container;
        [FormerlySerializedAs("AbilityBlock_2_Container")]
        [Required]
        public Transform AbilityBlock_1_Container;
        [FormerlySerializedAs("AbilityBlock_3_Container")]
        [Required]
        public Transform AbilityBlock_2_Container;
        [Required]
        public Transform AbilityBlock_OneShot_Container;
        [Required]
        public GameObject SelectOneCardHeaderGO;
        [Required]
        public Localize SelectOneCardHeaderText;
        [Required]
        public GameObject DivideLine;

        public Transform CountCardBlock;

        [Header("Localizations")]
        [SerializeField]
        private LocalizedString _chooseOneCardHeaderLoc;
        [SerializeField]
        private LocalizedString _useOneCardHeaderLoc;
        
        [Header("VFX")]
        public GameObject VFXIsInteractiveCard;
        public GameObject VFXFlashOutlineCard;
        
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
    }
}