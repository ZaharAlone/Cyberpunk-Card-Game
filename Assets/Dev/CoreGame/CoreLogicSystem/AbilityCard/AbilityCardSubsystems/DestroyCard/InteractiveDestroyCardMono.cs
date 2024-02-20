using System;
using System.Threading.Tasks;
using DG.Tweening;
using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CyberNet.Core.AbilityCard.DestroyCard
{
    public class InteractiveDestroyCardMono : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private string _guid;
        [SerializeField]
        private CardMono _cardMono;
        [SerializeField]
        private GameObject _destroyCardImage;
        [SerializeField]
        private GameObject _cardFaceObject;

        [Header("Icons card")]
        [SerializeField]
        private GameObject _iconsHandCard;
        [SerializeField]
        private GameObject _iconsDiscardCard;

        [Header("Animations")]
        [SerializeField]
        private Image _destroyEffectImage;

        [SerializeField]
        private Texture2D _textureNoise_1;
        [SerializeField]
        private Texture2D _textureNoise_2;

        [SerializeField]
        private EventReference _soundDestroyCard;
        
        private Vector2 _currentPointerPos;
        private bool _isDisableInteractive;
        private Sequence _sequence;

        public void OnEnable()
        {
            _cardFaceObject.SetActive(true);
            _destroyCardImage.SetActive(false);
            _destroyEffectImage.gameObject.SetActive(false);

            var material = _destroyEffectImage.material;
            material.SetFloat("_Outline", 2);
            _destroyEffectImage.material = material;
        }

        public void SetGUID(string guid)
        {
            _guid = guid;
        }

        public void DisableInteractive()
        {
            _isDisableInteractive = true;
        }
        
        public void SetIconsIsHand(bool isHand)
        {
            _iconsHandCard.SetActive(isHand);
            _iconsDiscardCard.SetActive(!isHand);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_isDisableInteractive)
                return;
            
            DestroyCardAction.SelectCard?.Invoke(_guid);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isDisableInteractive)
                return;
            DestroyCardAction.DeselectCard?.Invoke(_guid);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isDisableInteractive)
                return;
            
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            _currentPointerPos = eventData.pressPosition;
            DestroyCardAction.StartMoveCard?.Invoke(_guid);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_isDisableInteractive)
                return;
            
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            
            DestroyCardAction.EndMoveCard?.Invoke(_guid);
        }

        public void OnDestroyCardEffect()
        {
            _destroyCardImage.SetActive(true);
        }

        public void OffDestroyCardEffect()
        {
            _destroyCardImage.SetActive(false);
        }

        public void AnimationsDestroy()
        {
            _destroyEffectImage.material.SetFloat("_NoiseStrength", 0f);
            _destroyEffectImage.material.SetTexture("_NoiseMap", _textureNoise_1);
            _destroyEffectImage.gameObject.SetActive(true);
            
            RuntimeManager.CreateInstance(_soundDestroyCard).start();
            
            _sequence = DOTween.Sequence();
            _sequence.Append(_destroyEffectImage.material.DOFloat(1f, "_NoiseStrength", 1f))
                .OnComplete(() => AnimationsDestroyCardTwoStep());
        }

        private async void AnimationsDestroyCardTwoStep()
        {
            _cardFaceObject.SetActive(false);
            _destroyCardImage.SetActive(false);
            _destroyEffectImage.material.SetTexture("_NoiseMap", _textureNoise_2);

            await Task.Delay(100);
            
            _sequence.Kill();
            _sequence = DOTween.Sequence();
            _sequence.Append(_destroyEffectImage.material.DOFloat(-0.1f, "_NoiseStrength", 1f))
                .OnComplete(() => EndAnimationsDestroyCard());
        }

        private void EndAnimationsDestroyCard()
        {
            Debug.LogError("End animations");
        }
    }
}