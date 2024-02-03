using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace CyberNet.Meta.SelectPlayersForGame
{
    public class SelectImageUIMono : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public UnityEvent OnEnterImage;
        public UnityEvent OnExitImage;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            OnEnterImage?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnExitImage?.Invoke();
        }
    }
}