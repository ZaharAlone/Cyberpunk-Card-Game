using UnityEngine;
using UnityEngine.Serialization;
using Object = System.Object;

namespace CyberNet.Core.UI
{
    public class ViewDeckCardUIMono : MonoBehaviour
    {
        public GameObject PanelViewCard;
        public Transform ViewHandCardContainer;

        public void SetOpenWindow()
        {
            PanelViewCard.SetActive(true);
        }

        public void SetCardInContainer(GameObject cardObject)
        {
            var card = Instantiate(cardObject, ViewHandCardContainer);
            card.SetActive(true);
        }

        private void ClearCardContainer()
        {
            foreach (Transform child in ViewHandCardContainer)
            {
                Destroy(child.gameObject);
            }
        }

        public void OnCloseView()
        {
            ShowViewDeckCardAction.CloseView?.Invoke();
            PanelViewCard.SetActive(false);
            ClearCardContainer();
        }
    }
}