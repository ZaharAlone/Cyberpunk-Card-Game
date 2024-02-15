using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

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
        
        [Header("Icons card")]
        [SerializeField]
        private GameObject _iconsHandCard;
        [SerializeField]
        private GameObject _iconsDiscardCard;
        
        private Vector2 _currentPointerPos;
        private bool _isDisableInteractive;
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
    }
}