using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CyberNet.Core.Traderow
{
    public class TraderowMono : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public RectTransform TraderowContainerForCard;

        private Sequence _sequence;

        public void Start()
        {
            _sequence = DOTween.Sequence();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            TraderowUIAction.ShowTraderow?.Invoke();
        }

        public void ShowTraderow()
        {
            _sequence.Append(TraderowContainerForCard.DOAnchorPos(new Vector2(0, 0), 0.5f));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TraderowUIAction.HideTraderow?.Invoke();
        }

        public void HideTraderow()
        {
            _sequence.Append(TraderowContainerForCard.DOAnchorPos(new Vector2(0, 250), 0.5f));
        }
    }
}