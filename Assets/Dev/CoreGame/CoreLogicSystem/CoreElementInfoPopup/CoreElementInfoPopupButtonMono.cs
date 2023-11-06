using UnityEngine;
using UnityEngine.EventSystems;

namespace CyberNet.Core.UI.CorePopup
{
    public class CoreElementInfoPopupButtonMono : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private string _key;
        [SerializeField]
        private RectTransform _rectTransform;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            CoreElementInfoPopupAction.OpenPopupButton?.Invoke(_rectTransform, _key);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CoreElementInfoPopupAction.ClosePopupCard?.Invoke();
        }
    }
}