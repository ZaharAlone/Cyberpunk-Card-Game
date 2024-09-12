using System;
using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CyberNet.Core.InteractiveCard
{
    public class InteractiveCardMono : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public string GUID;
        public Vector2 CurrentPointerPos { get; private set; }

        private bool _isMoveCard;

        public void OnPointerEnter(PointerEventData eventData)
        {
            InteractiveActionCard.SelectCard?.Invoke(GUID);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isMoveCard)
                return;
            
            InteractiveActionCard.DeselectCard?.Invoke(GUID);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (_isMoveCard)
                return;
            
            CurrentPointerPos = eventData.pressPosition;
            InteractiveActionCard.StartInteractiveCard?.Invoke(GUID);

            _isMoveCard = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_isMoveCard)
                return;
            
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            _isMoveCard = false;
            InteractiveActionCard.EndInteractiveCard?.Invoke();
        }
    }
}