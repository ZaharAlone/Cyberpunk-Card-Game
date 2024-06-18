using System.Threading.Tasks;
using CyberNet.Global.Sound;
using DG.Tweening;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CyberNet.Core.Traderow
{
    public class TraderowMono : MonoBehaviour, IPointerExitHandler
    {
        [SerializeField]
        private TextMeshProUGUI _tradeValueText;
        [SerializeField]
        private ParticleSystem _effectAddEuroDollar;
        [SerializeField]
        private EventReference _addEuroDollarSFX; 
        
        public RectTransform TraderowContainer;
        public RectTransform TraderowContainerForCard;

        private Sequence _sequence;
        private float _timeAnimations = 0.5f;

        private float _lastClickTime;
        private const float delayBetweenClickHandling = 0.2f;
        
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

        public void PlayEffectAddTradePoint()
        {
            if (Time.unscaledTime - _lastClickTime - delayBetweenClickHandling <= float.Epsilon)
                return;
            
            _lastClickTime = Time.unscaledTime;
            
            _effectAddEuroDollar.Play();
            SoundAction.PlaySound?.Invoke(_addEuroDollarSFX);
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

        public void HideTraderowAnimations()
        {
            _sequence.Append(TraderowContainer.DOAnchorPos(new Vector2(0, 320), _timeAnimations));
        }

        public void ForceFullHidePanel()
        {
            TraderowContainer.anchoredPosition = new Vector2(0, 400);
        }

        public void SetEnableTradeRow(bool status)
        {
            TraderowContainer.gameObject.SetActive(status);
        }

        public void ShowPanelBaseViewAnimations()
        {
            _sequence.Append(TraderowContainer.DOAnchorPos(new Vector2(0, 320), _timeAnimations));
        }

        public void HideFullPanelAnimations()
        {
            _sequence.Append(TraderowContainer.DOAnchorPos(new Vector2(0, 400), _timeAnimations));
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            TraderowUIAction.HideTraderow?.Invoke();
        }
    }
}