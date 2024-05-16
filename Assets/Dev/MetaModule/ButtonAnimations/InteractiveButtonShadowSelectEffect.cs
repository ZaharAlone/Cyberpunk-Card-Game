using System;
using CyberNet.Global.Sound;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using FMODUnity;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace CyberNet.Meta
{
    public class InteractiveButtonShadowSelectEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler/*, ISelectHandler, IDeselectHandler*/
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
        [SerializeField]
        private UnityEvent _buttonSelectEvent;
        [SerializeField]
        private UnityEvent _buttonDeselectEvent;
        
        [Required]
        [SerializeField]
        private RectTransform _upTextRectEffect;
        [Required]
        [SerializeField]
        private TextMeshProUGUI _upTextEffect;
        [Required]
        [SerializeField]
        private RectTransform _downTextRectEffect;
        [Required]
        [SerializeField]
        private TextMeshProUGUI _downTextEffect;

        private Color32 _startEffectColor = new Color32(0, 181, 255, 25);
        private Color32 _finishEffectColor = new Color32(0, 181, 255, 60);
        private float _deltaStartPositionY = 25;
        
        #if UNITY_EDITOR
        [Button("Заполнить кнопку")]
        public void GetButtonComponent()
        {
            _button = GetComponent<Button>();

            foreach (Transform child in transform)
            {
                if (child.name == "txt_button_effect_up")
                {
                    _upTextRectEffect = child.GetComponent<RectTransform>();
                    _upTextEffect = child.GetComponent<TextMeshProUGUI>();
                    _upTextEffect.color = _startEffectColor;
                    _upTextRectEffect.anchoredPosition = new Vector2(_upTextRectEffect.anchoredPosition.x, _deltaStartPositionY);

                }
                if (child.name == "txt_button_effect_down")
                {
                    _downTextRectEffect = child.GetComponent<RectTransform>();
                    _downTextEffect = child.GetComponent<TextMeshProUGUI>();
                    _downTextEffect.color = _startEffectColor;
                    _downTextRectEffect.anchoredPosition = new Vector2(_downTextRectEffect.anchoredPosition.x, -_deltaStartPositionY);
                }
            }
        }
        #endif
        
        public void Start()
        {
            _button.onClick.AddListener(OnClicked);
        }

        public void OnEnable()
        {
            _upTextRectEffect.gameObject.SetActive(false);
            _downTextRectEffect.gameObject.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SoundAction.PlaySound?.Invoke(_soundButtonSelect);
            SelectAnimations();
            _buttonSelectEvent?.Invoke();
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            DeselectAnimations();
            _buttonDeselectEvent?.Invoke();
        }

        private void SelectAnimations()
        {
            _upTextRectEffect.anchoredPosition = new Vector2(_upTextRectEffect.anchoredPosition.x, _deltaStartPositionY);
            _downTextRectEffect.anchoredPosition = new Vector2(_downTextRectEffect.anchoredPosition.x, -_deltaStartPositionY);
            _upTextEffect.color = _startEffectColor;
            _downTextEffect.color = _startEffectColor;
            
            var sequence = DOTween.Sequence();

            sequence.Append(_upTextRectEffect.DOAnchorPosY(2, 0.45f))
                .Join(_upTextEffect.DOColor(_finishEffectColor, 0.45f))
                .Join(_downTextRectEffect.DOAnchorPosY(-2, 0.45f))
                .Join(_upTextEffect.DOColor(_finishEffectColor, 0.45f));
            
            _upTextRectEffect.gameObject.SetActive(true);
            _downTextRectEffect.gameObject.SetActive(true);
        }

        private void DeselectAnimations()
        {
            _upTextRectEffect.gameObject.SetActive(false);
            _downTextRectEffect.gameObject.SetActive(false);
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