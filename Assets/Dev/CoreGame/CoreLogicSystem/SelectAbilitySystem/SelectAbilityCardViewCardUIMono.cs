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
        public TextMeshProUGUI TextPriceCard;
        public TextMeshProUGUI TextLeftPoint;
        public TextMeshProUGUI TextRightPoint;
        public Transform AbilityContainer;

        public void SetViewCard(Sprite cardImage, string header, int price, int countLeftPoint, int countRightPoint)
        {
            ImageCard.sprite = cardImage;
            TextHeader.Term = header;
            
            TextPriceCard.text = price.ToString();
            TextLeftPoint.text = countLeftPoint.ToString();
            TextRightPoint.text = countRightPoint.ToString();
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