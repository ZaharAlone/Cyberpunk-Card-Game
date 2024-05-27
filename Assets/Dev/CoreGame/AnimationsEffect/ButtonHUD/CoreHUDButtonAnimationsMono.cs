using System;
using System.Threading.Tasks;
using Animancer;
using CyberNet.Core.UI.CorePopup;
using CyberNet.Global.Sound;
using FMODUnity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CyberNet.Core.UI
{
    public class CoreHUDButtonAnimationsMono : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private Button _actionButton;

        [SerializeField]
        private CoreElementInfoPopupButtonMono _popupButtonMono;
        
        [Header("Animations")]
        [SerializeField]
        private AnimancerComponent _animancer;
    
        [SerializeField]
        private AnimationClip _hide_button;
        [SerializeField]
        private AnimationClip _idle_animations;
        [SerializeField]
        private AnimationClip _ready_click_animations;
        [SerializeField]
        private AnimationClip _select_button_animations;

        [Header("Sound")]
        [SerializeField]
        private EventReference _select_button_sfx;
        [SerializeField]
        private EventReference _click_button_sfx;
        
        [Header("Logic")]
        [SerializeField]
        private UnityEvent _buttonClickEvent;

        private bool _isReadyClick;
        private const float fade_duration_animations = 0.5f;
        private const float fade_fast_duration_animations = 0.1f;
        private const float _delayBetweenClickHandling = 0.35f;
        
        private float _lastClickTime;
        private bool _isActivateButton;

        public void Start()
        {
            _actionButton.onClick.AddListener(ClickButton);
        }

        public void OnEnable()
        {
            _animancer.Play(_idle_animations, fade_duration_animations);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SoundAction.PlaySound?.Invoke(_select_button_sfx);
            _animancer.Play(_select_button_animations, fade_duration_animations);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isReadyClick && _ready_click_animations != null)
                _animancer.Play(_ready_click_animations, fade_duration_animations);
            else
                _animancer.Play(_idle_animations, fade_duration_animations);
        }

        public void SetAnimationsReadyClick()
        {
            if (_ready_click_animations == null)
                return;
            
            _isReadyClick = true;
            _animancer.Play(_ready_click_animations, fade_duration_animations);
        }

        public void SetAnimationsNotReadyButtonClick()
        {
            _isReadyClick = false;
            _animancer.Play(_idle_animations, fade_duration_animations);
        }

        private async void ClickButton()
        {
            if (Time.unscaledTime - _lastClickTime - _delayBetweenClickHandling <= float.Epsilon)
                return;

            _lastClickTime = Time.unscaledTime;
            
            SoundAction.PlaySound?.Invoke(_click_button_sfx);
            _animancer.Play(_hide_button, fade_fast_duration_animations);

            await Task.Delay(330);
            _buttonClickEvent?.Invoke();
        }

        public void OnDisable()
        {
            _isReadyClick = false;
        }

        public void OnDestroy()
        {
            _actionButton.onClick.RemoveListener(ClickButton);
        }
    }
}