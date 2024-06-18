using System.Collections.Generic;
using Animancer;
using CyberNet.Global.Sound;
using FMODUnity;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CyberNet.Core.UI
{
    public class DeckButtonViewMono : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        [Required]
        private Button _actionButton;
        
        [SerializeField]
        [Required]
        private AnimancerComponent _animancer;
        
        [SerializeField]
        [Required]
        private AnimationClip _idle_animations;
        [SerializeField]
        [Required]
        private AnimationClip _select_button_animations;

        [SerializeField]
        [Required]
        private EventReference _select_button_sfx;
        [SerializeField]
        [Required]
        private EventReference _click_button_sfx;
        
        [SerializeField]
        [Required]
        private UnityEvent _buttonClickEvent;

        [SerializeField]
        private List<Image> _cardImagesAnimations = new List<Image>();
        [SerializeField]
        [Required]
        private TextMeshProUGUI _countCardText;
        [SerializeField]
        [Required]
        private TextMeshProUGUI _nameDeckText;

        [SerializeField]
        private Color32 _defaultDeckColor;
        [SerializeField]
        private Color32 _emptyDeckColor;

        private const float fade_duration_animations = 0.25f;
        private const float _delayBetweenClickHandling = 0.35f;
        
        private float _lastClickTime;
        private bool _isActivateButton;
        
        public void Start()
        {
            _actionButton.onClick.AddListener(ClickButton);
        }

        public void SetCountCardInDeck(int count)
        {
            _countCardText.text = count.ToString();
            
            if (count == 0)
                RecolorDeck(_emptyDeckColor);
            else
                RecolorDeck(_defaultDeckColor);
        }

        private void RecolorDeck(Color32 targetColor)
        {
            _countCardText.color = targetColor;
            _nameDeckText.color = targetColor;
            
            foreach (var imageAnimationCard in _cardImagesAnimations)
            {
                imageAnimationCard.color = targetColor;
            }
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
            _animancer.Play(_idle_animations, fade_duration_animations);
        }

        private void ClickButton()
        {
            if (Time.unscaledTime - _lastClickTime - _delayBetweenClickHandling <= float.Epsilon)
                return;

            _lastClickTime = Time.unscaledTime;
            
            SoundAction.PlaySound?.Invoke(_click_button_sfx);
            _buttonClickEvent?.Invoke();
        }

        public void OnDestroy()
        {
            _actionButton.onClick.RemoveListener(ClickButton);
        }
    }
}