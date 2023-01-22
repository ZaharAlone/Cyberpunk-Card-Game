using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BoardGame.Core
{
    public class InteractiveCard : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public string GUID;

        public Vector2 CurrentPointerPos { get; private set; }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            CurrentPointerPos = eventData.pressPosition;
            InteractiveActionCard.StartInteractiveCard?.Invoke(GUID);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            InteractiveActionCard.EndInteractiveCard?.Invoke();
        }
    }
}