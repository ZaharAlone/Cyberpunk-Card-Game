using System.Threading.Tasks;
using CyberNet.Global.Sound;
using DG.Tweening;
using FMODUnity;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

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
        
        [Required]
        public RectTransform TraderowContainer;
        [Required]
        public RectTransform TraderowContainerForCard;

        private Sequence _sequence;
        private float _timeAnimations = 0.5f;
        private float _timeHidePanelAnimations = 0.25f;

        private float _lastClickTime;
        private const float delayBetweenClickHandling = 0.2f;

        private Vector2 show_mini_panel_positions = new Vector2(0, 320);
        private Vector2 full_hide_positions = new Vector2(0, 400);
        private Vector2 full_show_positions = new Vector2(0, 0);
        
        public void Start()
        {
            _sequence = DOTween.Sequence();
        }

        public void DisableTradeRow()
        {
            TraderowContainer.gameObject.SetActive(false);
        }

        public void EnableTradeRow()
        {
            TraderowContainer.gameObject.SetActive(true);
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

        public void ShowFullTradeRowPanelAnimations()
        {
            _sequence.Append(TraderowContainer
                .DOAnchorPos(full_show_positions, _timeAnimations))
                .OnComplete(()=> EndShowAnimations()); 
        }

        private void EndShowAnimations()
        {
            TraderowUIAction.EndShowAnimations?.Invoke();
        }

        public void TradeRowToMiniPanelAnimations()
        {
            _sequence.Append(TraderowContainer.DOAnchorPos(show_mini_panel_positions, _timeAnimations));
        }

        public void FullHideTradeRowAnimations()
        {
            _sequence.Append(TraderowContainer.DOAnchorPos(full_hide_positions, _timeHidePanelAnimations));
        }

        public void ForceFullHidePanel()
        {
            TraderowContainer.anchoredPosition = full_hide_positions;
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            TraderowUIAction.HideTraderow?.Invoke();
        }
    }
}