using CyberNet.Core.Battle.TacticsMode;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace CyberNet.Core.Battle.TacticsMode.InteractiveCard
{
    public class InteractiveCardTacticsScreenMono : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Required]
        public CardMono CardMono;
        [SerializeField]
        private string _guid;

        private bool _isDisableInteractive;
        private bool _isSelectCard;
        private Sequence _sequence;
        
        public void SetGUID(string guid)
        {
            _guid = guid;
        }

        public void DisableInteractive()
        {
            _isDisableInteractive = true;
        }

        public void DisableSelectCard()
        {
            _isSelectCard = false;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_isDisableInteractive)
                return;

            _isSelectCard = true;

            BattleTacticsUIAction.SelectCard?.Invoke(_guid);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isDisableInteractive)
                return;
            
            if (!_isSelectCard)
                return;
            
            BattleTacticsUIAction.DeselectCard?.Invoke(_guid);
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isDisableInteractive)
                return;
            
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            
            BattleTacticsUIAction.StartMoveCard?.Invoke(_guid);
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            if (_isDisableInteractive)
                return;
            
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            
            BattleTacticsUIAction.EndMoveCard?.Invoke(_guid);
            _isSelectCard = false;
        }
    }
}