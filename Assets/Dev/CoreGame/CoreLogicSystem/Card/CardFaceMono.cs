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
        public Transform AbilityBlock_0_Container;
        [Required]
        public Transform AbilityBlock_1_Container;
        [Required]
        public Transform AbilityBlock_OneShot_Container;
        [Required]
        public GameObject SelectOneCardHeaderGO;
        [Required]
        public Localize SelectOneCardHeaderText;

        public Transform CountCardBlock;

        [Header("Battle Point")]
        [Required]
        public TextMeshProUGUI LeftPointText;
        [Required]
        public TextMeshProUGUI RightPointText;
        
        [Header("VFX")]
        [SerializeField]
        [Required]
        private GameObject _vfxIsInteractiveCard;
        [SerializeField]
        [Required]
        public GameObject _vfxCardInTargetZone;

        [Header("Discard Card")]
        [SerializeField]
        [Required]
        private GameObject _discardCardEffectGO;
        
        public void SetViewCard(Sprite imageCard, string header, int leftPointCount, int rightPointCount, int price = 0)
        {
            Header.Term = header;
            ImageCard.sprite = imageCard;
            LeftPointText.text = leftPointCount.ToString();
            RightPointText.text = rightPointCount.ToString();

            if (price != 0)
                PriceText.text = price.ToString();
            else
                PriceText.gameObject.SetActive(false);
        }

        public void SetHeaderSelectAbility(bool status)
        {
            SelectOneCardHeaderGO.SetActive(status);
        }

        public void SetBigDownBlock()
        {
            ImageDownBlockRect.sizeDelta = new Vector2(ImageDownBlockRect.sizeDelta.x, 120f);
            AbilityBlockRect.anchoredPosition = new Vector2(AbilityBlockRect.anchoredPosition.x, 120f);
        }

        public void VFXCardReadyToInteractive()
        {
            _vfxIsInteractiveCard.SetActive(true);
            _vfxCardInTargetZone.SetActive(false);
        }

        public void VFXDisable()
        {
            _vfxIsInteractiveCard.SetActive(false);
            _vfxCardInTargetZone.SetActive(false);
        }
        
        public void VFXCardInTargetZone()
        {
            _vfxIsInteractiveCard.SetActive(false);
            _vfxCardInTargetZone.SetActive(true);
        }
        
        public void EnableDiscardCardEffect(bool status)
        {
            _discardCardEffectGO.SetActive(status);
        }

        public void HideDownBlock()
        {
            ImageDownBlockRect.gameObject.SetActive(false);
        }
    }
}