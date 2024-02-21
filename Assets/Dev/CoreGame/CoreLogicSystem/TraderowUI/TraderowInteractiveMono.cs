using UnityEngine;
using UnityEngine.EventSystems;

namespace CyberNet.Core.Traderow
{
    public class TraderowInteractiveMono : MonoBehaviour, IPointerEnterHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            TraderowUIAction.ShowTraderow?.Invoke();
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            TraderowUIAction.HideTraderow?.Invoke();
        }
    }
}