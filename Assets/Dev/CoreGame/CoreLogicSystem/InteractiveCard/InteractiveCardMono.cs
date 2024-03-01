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

        public void OnPointerEnter(PointerEventData eventData)
        {
            InteractiveActionCard.SelectCard?.Invoke(GUID);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            InteractiveActionCard.DeselectCard?.Invoke(GUID);
        }

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