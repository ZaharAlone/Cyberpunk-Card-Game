using System.Collections;
using System.Collections.Generic;
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
    public class InteractiveButtonHideShowElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Required]
        public Button Button;

        public GameObject ActiveButton;
        public GameObject DeactiveButton;

        public Image ImageActiveButton;
        
        public TextMeshProUGUI ButtonTextActive;
        public TextMeshProUGUI ButtonTextDeactive;
        
        public Localize ButtonTextActiveLoc;
        public Localize ButtonTextDeactiveLoc;
        
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
        }

        public void SetText(string text)
        {
            ButtonTextActive.text = text;
            ButtonTextDeactive.text = text;
        }

        public void SetLocalizeTerm(string locTerm)
        {
            ButtonTextActiveLoc.Term = locTerm;
            ButtonTextDeactiveLoc.Term = locTerm;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Time.unscaledTime - _lastClickTime - _delayBetweenClickHandling <= float.Epsilon)
                return;

            ActiveButton.SetActive(true);
            DeactiveButton.SetActive(false);
            RuntimeManager.CreateInstance(SoundButtonSelect).start();
            _sequence = DOTween.Sequence();
            _sequence.Append(ImageActiveButton.DOColor(new Color32(255, 255, 255, 255), 0.25f));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (Time.unscaledTime - _lastClickTime - _delayBetweenClickHandling <= float.Epsilon)
                return;

            _sequence = DOTween.Sequence();
            _sequence.Append(ImageActiveButton.DOColor(new Color32(255, 255, 255, 0), 0.25f)).OnComplete(DeactivateButtonAction);
        }

        public void ActivateButton()
        {
            ActiveButton.SetActive(true);
        }

        private void DeactivateButtonAction()
        {
            ActiveButton.SetActive(false);
            DeactiveButton.SetActive(true);
        }
        
        public void OnClicked()
        {
            RuntimeManager.CreateInstance(SoundButtonClick).start();
            ButtonClickEvent?.Invoke();
        }
        
        public void OnDisable()
        {
            if (_sequence != null)
                _sequence.Kill();
        }
    }
}