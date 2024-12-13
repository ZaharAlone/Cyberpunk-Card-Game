using CyberNet.Global.Sound;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using FMODUnity;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace CyberNet.Meta
{
    public class InteractiveButtonNotAnimations : MonoBehaviour, IPointerEnterHandler/*, ISelectHandler, IDeselectHandler*/
    {
        [SerializeField]
        [Required]
        private Button _button;

        [SerializeField]
        private EventReference _soundButtonClick;
        [SerializeField]
        private EventReference _soundButtonSelect;
        [SerializeField]
        private UnityEvent _buttonClickEvent;
        
        #if UNITY_EDITOR
        [Button("Заполнить кнопку")]
        public void GetButtonComponent()
        {
            _button = this.GetComponent<Button>();
        }
        #endif
        
        public void Start()
        {
            _button.onClick.AddListener(OnClicked);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SoundAction.PlaySound?.Invoke(_soundButtonSelect);
        }
        
        public void OnSelect(BaseEventData eventData)
        {
            OnClicked();
        }

        public void OnDeselect(BaseEventData eventData)
        {
        }
        
        public void OnClicked()
        {
            SoundAction.PlaySound?.Invoke(_soundButtonClick);
            _buttonClickEvent?.Invoke();
        }
    }
}