using CyberNet.Core.Battle.TacticsMode;
using CyberNet.Core.InteractiveCard;
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

            BattleTacticsUIAction.SelectCardTactics?.Invoke(_guid);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isDisableInteractive)
                return;
            
            if (!_isSelectCard)
                return;
            
            BattleTacticsUIAction.DeselectCardTactics?.Invoke(_guid);
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isDisableInteractive)
                return;
            
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            
            BattleTacticsUIAction.StartMoveCardTactics?.Invoke(_guid);
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            if (_isDisableInteractive)
                return;
            
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            
            BattleTacticsUIAction.EndMoveCardTactics?.Invoke();
            _isSelectCard = false;
        }
    }
}