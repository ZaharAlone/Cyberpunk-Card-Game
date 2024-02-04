using UnityEngine;
using TMPro;
using UnityEngine.UI;
using I2.Loc;
using Sirenix.OdinInspector;
using DG.Tweening;
using FMODUnity;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace CyberNet.Meta
{
    public class InteractiveButtonHideShowElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        [Required]
        public Button Button;

        [SerializeField]
        private bool _isFirstButton;
        
        [SerializeField]
        private GameObject _selectButton;

        public TextMeshProUGUI ButtonTextDeactive;
        
        public EventReference SoundButtonClick;
        public EventReference SoundButtonSelect;
        public UnityEvent ButtonClickEvent;
        
        private const float _delayBetweenClickHandling = 0.35f;

        private Sequence _sequence;
        private float _lastClickTime;
        private bool _isActivateButton;

        public void Start()
        {
            Button.onClick.AddListener(OnClicked);
            
            if (_isFirstButton)
            {
                Button.Select();
                //SelectButton();
            }
        }

        public void SetText(string text)
        {
            ButtonTextDeactive.text = text;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Time.unscaledTime - _lastClickTime - _delayBetweenClickHandling <= float.Epsilon)
                return;
            
            OnSelectButton();
        }

        private void OnSelectButton()
        {
            _selectButton.SetActive(true);
            RuntimeManager.CreateInstance(SoundButtonSelect).start();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (Time.unscaledTime - _lastClickTime - _delayBetweenClickHandling <= float.Epsilon)
                return;
            
            DeselectButton();
        }

        private void DeselectButton()
        {
            _selectButton.SetActive(false);
        }
        
        public void OnSelect(BaseEventData eventData)
        {
            OnSelectButton();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            DeselectButton();
        }

        public void ActivateButton()
        {
            _selectButton.SetActive(true);
        }

        private void DeactivateButtonAction()
        {
            _selectButton.SetActive(false);
        }
        
        public void OnClicked()
        {
            RuntimeManager.CreateInstance(SoundButtonClick).start();
            ButtonClickEvent?.Invoke();
            _selectButton.SetActive(false);
        }
        
        public void OnDisable()
        {
            if (_sequence != null)
                _sequence.Kill();
        }
    }
}