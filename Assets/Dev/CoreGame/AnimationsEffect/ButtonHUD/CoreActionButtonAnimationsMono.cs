using System.Collections.Generic;
using System.Threading.Tasks;
using Animancer;
using CyberNet.Core.Arena.ArenaHUDUI;
using CyberNet.Core.UI.ActionButtonAnimations;
using CyberNet.Core.UI.CorePopup;
using CyberNet.Global.Sound;
using FMODUnity;
using I2.Loc;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CyberNet.Core.UI.ActionButton
{
    public class CoreActionButtonAnimationsMono : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private Button _actionButton;
        [Required]
        [SerializeField]
        private Localize _actionButtonLocText;
        [SerializeField]
        private List<Image> _graphicsButtonElements = new List<Image>();
        [SerializeField]
        private TextMeshProUGUI _actionButtonText;

        [SerializeField]
        private Color32 _hideColor;

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

        [Header("Action Buttons")]
        [Required]
        [SerializeField]
        private GameObject _playAllActionButtonGO;
        [Required]
        [SerializeField]
        private ActionButtonMoveAnimationsMono _playAllActionButtonAnimations;
        
        [Required]
        [SerializeField]
        private GameObject _endRoundActionButtonGO;
        [Required]
        [SerializeField]
        private ActionButtonMoveAnimationsMono _endRoundActionButtonAnimations;
        
        [Required]
        [SerializeField]
        private GameObject _attackActionButtonGO;
        [Required]
        [SerializeField]
        private ActionButtonScaleAnimationsMono _attackRoundActionButtonAnimations;

        [Header("Localize")]
        [SerializeField]
        private LocalizedString _playAllCardLocalize;
        [SerializeField]
        private LocalizedString _endRoundLocalize;
        [SerializeField]
        private LocalizedString _attackLocalize;
        
        [Header("Sound")]
        [SerializeField]
        private EventReference _select_button_sfx;
        [SerializeField]
        private EventReference _click_button_sfx;
        
        [HideInInspector]
        public bool IsEnableButton;
        
        private bool _isReadyClick;
        private const float fade_duration_animations = 0.5f;
        private const float fade_fast_duration_animations = 0.1f;
        private const float _delayBetweenClickHandling = 0.35f;
        
        private float _lastClickTime;
        private bool _isActivateButton;
        private ActionPlayerButtonType _currentStateVisualActionButton;

        public void Start()
        {
            _actionButton.onClick.AddListener(ClickButton);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsEnableButton)
                return;
            
            SoundAction.PlaySound?.Invoke(_select_button_sfx);
            _animancer.Play(_select_button_animations, fade_duration_animations);
            StartIconsActionAnimations();
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!IsEnableButton)
                return;
            
            if (_isReadyClick && _ready_click_animations != null)
                _animancer.Play(_ready_click_animations, fade_duration_animations);
            else
            {
                _animancer.Play(_idle_animations, fade_duration_animations);
                StopIconsActionAnimations();
            }
        }

        public void SetStateViewButton(ActionPlayerButtonType actionButtonState)
        {
            _playAllActionButtonGO.SetActive(false);
            _endRoundActionButtonGO.SetActive(false);
            _attackActionButtonGO.SetActive(false);
            
            switch (actionButtonState)
            {
                case ActionPlayerButtonType.PlayAll:
                    _actionButtonLocText.Term = _playAllCardLocalize.mTerm;
                    _playAllActionButtonGO.SetActive(true);
                    break;
                case ActionPlayerButtonType.EndTurn:
                    _actionButtonLocText.Term = _endRoundLocalize.mTerm;
                    _endRoundActionButtonGO.SetActive(true);
                    break;
                case ActionPlayerButtonType.Attack:
                    _actionButtonLocText.Term = _attackLocalize.mTerm;
                    _attackActionButtonGO.SetActive(true);
                    break;
            }

            _currentStateVisualActionButton = actionButtonState;
        }
        
        public void SetAnimationsReadyClick()
        {
            _isReadyClick = true;
            _animancer.Play(_ready_click_animations, fade_duration_animations);
            StartIconsActionAnimations();
        }

        public void SetAnimationsNotReadyButtonClick()
        {
            _isReadyClick = false;
            _animancer.Play(_idle_animations, fade_duration_animations);
            StopIconsActionAnimations();
        }
        
        public void ShowButtonPlayAnimations()
        {
            IsEnableButton = true;
            _popupButtonMono.EnablePopup();
            _animancer.Play(_idle_animations, fade_duration_animations);
        }

        public void HideButtonPlayAnimations()
        {
            IsEnableButton = false;
            _isReadyClick = false;
            
            _popupButtonMono.DisablePopup();
            _animancer.Play(_hide_button, fade_duration_animations);
            HideIconsActionAnimations();
        }

        public void ForceHideActionButton()
        {
            IsEnableButton = false;
            _isReadyClick = false;
            _popupButtonMono.DisablePopup();

            foreach (var imageActionButton in _graphicsButtonElements)
            {
                imageActionButton.color = _hideColor;
            }

            _actionButtonText.color = _hideColor;
        }

        private void StartIconsActionAnimations()
        {
            switch (_currentStateVisualActionButton)
            {
                case ActionPlayerButtonType.PlayAll:
                    _playAllActionButtonAnimations.StartMoveAnimations();
                    break;
                case ActionPlayerButtonType.EndTurn:
                    _endRoundActionButtonAnimations.StartMoveAnimations();
                    break;
                case ActionPlayerButtonType.Attack:
                    _attackRoundActionButtonAnimations.StartScaleAnimations();
                    break;
            }
        }

        private void StopIconsActionAnimations()
        {
            switch (_currentStateVisualActionButton)
            {
                case ActionPlayerButtonType.PlayAll:
                    _playAllActionButtonAnimations.StopAnimations();
                    break;
                case ActionPlayerButtonType.EndTurn:
                    _endRoundActionButtonAnimations.StopAnimations();
                    break;
                case ActionPlayerButtonType.Attack:
                    _attackRoundActionButtonAnimations.StopAnimations();
                    break;
            }
        }

        private void HideIconsActionAnimations()
        {
            switch (_currentStateVisualActionButton)
            {
                case ActionPlayerButtonType.PlayAll:
                    _playAllActionButtonAnimations.HideIcons();
                    break;
                case ActionPlayerButtonType.EndTurn:
                    _endRoundActionButtonAnimations.HideIcons();
                    break;
                case ActionPlayerButtonType.Attack:
                    _attackRoundActionButtonAnimations.HideIcons();
                    break;
            }
        }
        
        private async void ClickButton()
        {
            if (Time.unscaledTime - _lastClickTime - _delayBetweenClickHandling <= float.Epsilon)
                return;

            _lastClickTime = Time.unscaledTime;
            
            SoundAction.PlaySound?.Invoke(_click_button_sfx);

            if (_currentStateVisualActionButton == ActionPlayerButtonType.EndTurn)
            {
                var isPlayerActionLeft = ActionPlayerButtonEvent.CheckPlayerHasAnyActionsLeft.Invoke();

                if (isPlayerActionLeft)
                    return;   
            }
            
            _animancer.Play(_hide_button, fade_fast_duration_animations);
            HideIconsActionAnimations();
            _popupButtonMono.ForceClosePopup();

            IsEnableButton = false;
            
            await Task.Delay(330);

            if (_currentStateVisualActionButton == ActionPlayerButtonType.Attack)
                ArenaUIAction.ClickAttack?.Invoke();
            else
                ActionPlayerButtonEvent.ClickActionButton?.Invoke();   
        }

        public void ForceHideButton()
        {
            _animancer.Play(_hide_button, fade_fast_duration_animations);
            HideIconsActionAnimations();
            _popupButtonMono.ForceClosePopup();

            IsEnableButton = false;
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