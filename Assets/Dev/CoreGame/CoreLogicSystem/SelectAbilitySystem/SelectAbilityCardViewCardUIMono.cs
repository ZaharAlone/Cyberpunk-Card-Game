using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace CyberNet.Core
{
    public class SelectAbilityCardViewCardUIMono: MonoBehaviour
    {
        public Image ImageCard;
        public Localize TextHeader;
        public Image ImageNations;
        public TextMeshProUGUI TextPriceCard;
        public TextMeshProUGUI TextDestroyCard;
        public Transform AbilityContainer;

        public void SetViewCard(Sprite cardImage, string header, int price, int destroy, Sprite nationsImage)
        {
            ImageCard.sprite = cardImage;
            TextHeader.Term = header;
            
            ImageNations.gameObject.SetActive(nationsImage != null);
            ImageNations.sprite = nationsImage;
            TextPriceCard.text = price.ToString();
            TextDestroyCard.text = destroy.ToString();
        }

        public void ClearViewCard()
        {
            foreach (Transform child in AbilityContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }
}