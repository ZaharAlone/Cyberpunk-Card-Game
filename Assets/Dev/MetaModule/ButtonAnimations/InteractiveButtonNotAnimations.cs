using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using FMODUnity;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace CyberNet.Meta
{
    public class InteractiveButtonNotAnimations : MonoBehaviour, IPointerEnterHandler
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
            RuntimeManager.CreateInstance(_soundButtonSelect).start();
        }
        
        public void OnClicked()
        {
            RuntimeManager.CreateInstance(_soundButtonClick).start();
            _buttonClickEvent?.Invoke();
        }
    }
}