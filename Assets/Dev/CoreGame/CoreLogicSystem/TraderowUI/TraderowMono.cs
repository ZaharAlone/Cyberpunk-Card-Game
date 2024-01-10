using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CyberNet.Core.Traderow
{
    public class TraderowMono : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private TextMeshProUGUI _tradeValueText;
        public RectTransform TraderowContainer;
        public RectTransform TraderowContainerForCard;

        private Sequence _sequence;
        private float _timeAnimations = 0.5f;
        
        public void Start()
        {
            _sequence = DOTween.Sequence();
        }

        public void HideTradeRow()
        {
            TraderowContainer.gameObject.SetActive(false);
        }

        public void ShowTradeRow()
        {
            TraderowContainer.gameObject.gameObject.SetActive(true);
        }
        
        public void SetTradeValue(int tradeValue)
        {
            _tradeValueText.text = tradeValue.ToString();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            TraderowUIAction.ShowTraderow?.Invoke();
        }

        public void ShowTraderowAnimations()
        {
            _sequence.Append(TraderowContainer
                .DOAnchorPos(new Vector2(0, 0), _timeAnimations));
            EndShowAnimations();
        }

        private async void EndShowAnimations()
        {
            var waitTime = (int)(_timeAnimations * 1000);
            await Task.Delay(waitTime);
            TraderowUIAction.EndShowAnimations?.Invoke();
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            TraderowUIAction.HideTraderow?.Invoke();
        }

        public void HideTraderowAnimations()
        {
            _sequence.Append(TraderowContainer.DOAnchorPos(new Vector2(0, 320), _timeAnimations));
        }

        public void ForceFullHidePanel()
        {
            TraderowContainer.anchoredPosition = new Vector2(0, 400);
        }

        public void ShowPanelBaseViewAnimations()
        {
            _sequence.Append(TraderowContainer.DOAnchorPos(new Vector2(0, 320), _timeAnimations));
        }

        public void HideFullPanelAnimations()
        {
            _sequence.Append(TraderowContainer.DOAnchorPos(new Vector2(0, 400), _timeAnimations));
        }
    }
}