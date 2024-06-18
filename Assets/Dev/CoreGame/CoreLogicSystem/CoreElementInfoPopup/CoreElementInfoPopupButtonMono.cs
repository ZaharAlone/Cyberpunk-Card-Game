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

        private bool _isDisablePopup;
        
        public void DisablePopup()
        {
            _isDisablePopup = true;
        }

        public void EnablePopup()
        {
            _isDisablePopup = false;
        }
        
        public void SetKeyPopup(string key)
        {
            _key = key;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_isDisablePopup)
                return;
            
            CoreElementInfoPopupAction.OpenPopupButton?.Invoke(_rectTransform, _key);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isDisablePopup)
                return;
            
            ClosePopup();
        }

        public void ForceClosePopup()
        {
            ClosePopup();
        }

        private void ClosePopup()
        {
            CoreElementInfoPopupAction.ClosePopupCard?.Invoke();
        }
    }
}