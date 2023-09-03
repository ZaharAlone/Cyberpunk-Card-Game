using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
namespace CyberNet.CoreGame.Traderow
{
    public class TraderowMono : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public RectTransform TrederowTransform;
        public RectTransform TraderowContainerForCard;

        private Sequence _sequence;

        public void Start()
        {
            _sequence = DOTween.Sequence();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _sequence.Append(TraderowContainerForCard.DOAnchorPos(new Vector2(0, 0), 0.5f));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _sequence.Append(TraderowContainerForCard.DOAnchorPos(new Vector2(0, 250), 0.5f));
        }
    }
}