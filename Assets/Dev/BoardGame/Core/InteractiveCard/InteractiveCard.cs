using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BoardGame.Core
{
    public class InteractiveCard : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public string GUID;

        private bool _isMoveStarted;
        public Vector2 CurrentPointerPos { get; private set; }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isMoveStarted = true;
            CurrentPointerPos = eventData.pressPosition;
            InteractiveActionCard.InteractiveCard?.Invoke(_isMoveStarted, GUID, eventData.button.ToString());
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isMoveStarted = false;
            InteractiveActionCard.InteractiveCard?.Invoke(_isMoveStarted, GUID, eventData.button.ToString());
        }
    }
}