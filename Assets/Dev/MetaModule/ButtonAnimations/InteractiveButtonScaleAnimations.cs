using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using FMODUnity;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace CyberNet.Meta
{
    public class InteractiveButtonScaleAnimations : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

        [Header("Blur button on select")]
        [SerializeField]
        private bool _onBlur;
        [SerializeField]
        private GameObject _blurButton;

        [Header("Scale animations")]
        [SerializeField]
        private float _scaleSelectButton = 1.15f;
        [SerializeField]
        private float _scaleClickButton = 0.85f;
        
        private Sequence _sequence;
        
        #if UNITY_EDITOR
        [Button("Заполнить кнопку")]
        public void GetButtonComponent()
        {
            _button = GetComponent<Button>();
        }
        #endif
        
        public void Start()
        {
            _button.onClick.AddListener(OnClicked);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            RuntimeManager.CreateInstance(_soundButtonSelect).start();

            _sequence = DOTween.Sequence();
            _sequence.Append(transform.DOScale(_scaleSelectButton, 0.15f));
            if (_onBlur)
                _blurButton.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _sequence = DOTween.Sequence();
            
            _sequence.Append(transform.DOScale(1f, 0.15f));
            if (_onBlur)
                _blurButton.SetActive(false);
        }
        
        public void OnClicked()
        {
            RuntimeManager.CreateInstance(_soundButtonClick).start();
            
            _sequence = DOTween.Sequence();
            _sequence.Append(transform.DOScale(_scaleClickButton, 0.3f))
                .Append(transform.DOScale(1f, 0.15f))
                .OnComplete(ClickEvent);
        }

        private void ClickEvent()
        {
            _buttonClickEvent?.Invoke();
        }

        public void OnDisable()
        {
            _sequence.Kill();
        }
    }
}