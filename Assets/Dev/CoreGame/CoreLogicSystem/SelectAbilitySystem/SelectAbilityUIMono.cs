using UnityEngine;
namespace CyberNet.Core
{
    public class SelectAbilityUIMono : MonoBehaviour
    {
        public GameObject Background;
        public GameObject Panel;

        public SelectAbilityCardViewCardUIMono LeftCard;
        public SelectAbilityCardViewCardUIMono RightCard;

        public void OpenFrame()
        {
            Background.SetActive(true);
            Panel.SetActive(true);
        }

        public void CloseFrame()
        {
            Background.SetActive(false);
            Panel.SetActive(false);
            
            LeftCard.ClearViewCard();
            RightCard.ClearViewCard();
        }
        
        public void SelectFirstAbility()
        {
            SelectAbilityAction.SelectFirstAbility?.Invoke();
        }

        public void SelectSecondAbility()
        {
            SelectAbilityAction.SelectSecondAbility?.Invoke();
        }
    }
}