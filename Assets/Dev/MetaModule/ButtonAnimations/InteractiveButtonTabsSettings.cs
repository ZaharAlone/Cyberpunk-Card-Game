using UnityEngine;
using TMPro;
using UnityEngine.UI;
using I2.Loc;
using Sirenix.OdinInspector;
using DG.Tweening;
using FMODUnity;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace CyberNet.Meta
{
    public class InteractiveButtonTabsSettings : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        [Required]
        [SerializeField]
        private Button _button;
        
        [SerializeField]
        private GameObject _activeButton;
        [SerializeField]
        private GameObject _selectButton;
        [SerializeField]
        private GameObject _deactiveButton;

        [SerializeField]
        private Image _imageActiveButton;
        
        [SerializeField]
        private EventReference _soundButtonClick;
        [SerializeField]
        private EventReference _soundButtonSelect;
        [SerializeField]
        private UnityEvent _buttonClickEvent;

        private bool _isActivateButton;
        private Sequence _sequence;

        public void Start()
        {
            _button.onClick.AddListener(OnClicked);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SelectButton();
        }

        private void SelectButton()
        {
            RuntimeManager.CreateInstance(_soundButtonSelect).start();

            if (!_isActivateButton)
            {
                _selectButton.SetActive(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            DeselectButton();
        }

        private void DeselectButton()
        {
            if (!_isActivateButton)
                _selectButton.SetActive(false);
        }
        
        public void OnSelect(BaseEventData eventData)
        {
            SelectButton();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            DeselectButton();
        }

        public void DeactivateButtonAnimation()
        {
            if (!_isActivateButton)
                return;
            
            _activeButton.SetActive(false);
            _deactiveButton.SetActive(true);
            _selectButton.SetActive(false);
            _isActivateButton = false;
            
            _sequence = DOTween.Sequence();
            _sequence.Append(_imageActiveButton.DOColor(new Color32(255, 255, 255, 0), 0.25f));
        }
        
        public void OnClicked()
        {
            _buttonClickEvent?.Invoke();
        }

        public void ActivateButtonAnimation()
        {
            RuntimeManager.CreateInstance(_soundButtonClick).start();
            
            if (_isActivateButton)
                return;
            
            _activeButton.SetActive(true);
            _deactiveButton.SetActive(false);
            _selectButton.SetActive(false);
            _sequence = DOTween.Sequence();
            _sequence.Append(_imageActiveButton.DOColor(new Color32(255, 255, 255, 255), 0.25f));
            
            _isActivateButton = true;
        }

        public void SetForceActivateButton()
        {
            _activeButton.SetActive(true);
            _imageActiveButton.color = Color.white;
            
            _deactiveButton.SetActive(false);
            _selectButton.SetActive(false);
            _isActivateButton = true;
        }
        
        public void SetForceDeactivateButton()
        {
            _activeButton.SetActive(false);
            
            _deactiveButton.SetActive(true);
            _selectButton.SetActive(false);
            _isActivateButton = false;
        }
        
        public void OnDisable()
        {
            if (_sequence != null)
                _sequence.Kill();
        }
    }
}