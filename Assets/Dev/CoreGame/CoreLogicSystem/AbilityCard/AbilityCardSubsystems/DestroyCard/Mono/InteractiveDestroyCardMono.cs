using System;
using System.Threading.Tasks;
using DG.Tweening;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace CyberNet.Core.AbilityCard.DestroyCard
{
    public class InteractiveDestroyCardMono : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private string _guid;
        [SerializeField]
        [Required]
        private CardMono _cardMono;
        [SerializeField]
        [Required]
        private GameObject _destroyCardImage;
        [SerializeField]
        [Required]
        private GameObject _cardFaceObject;

        [Header("Animations")]
        [SerializeField]
        [Required]
        private Image _destroyEffectImage;

        [SerializeField]
        [Required]
        private Texture2D _textureNoise_1;
        [SerializeField]
        [Required]
        private Texture2D _textureNoise_2;

        [SerializeField]
        private EventReference _soundDestroyCard;
        
        private bool _isDisableInteractive;
        private bool _isSelectCard;
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

        public void DisableSelectCard()
        {
            _isSelectCard = false;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_isDisableInteractive)
                return;

            _isSelectCard = true;

            DestroyCardAction.SelectCard?.Invoke(_guid);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isDisableInteractive)
                return;
            
            if (!_isSelectCard)
                return;
            
            DestroyCardAction.DeselectCard?.Invoke(_guid);
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isDisableInteractive)
                return;
            
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            
            DestroyCardAction.StartMoveCard?.Invoke(_guid);
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            if (_isDisableInteractive)
                return;
            
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            
            DestroyCardAction.EndMoveCard?.Invoke(_guid);
            _isSelectCard = false;
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
            DestroyCardAction.EndAnimationsDestroyCurrentCard?.Invoke();
            Destroy(this.gameObject);
        }
    }
}