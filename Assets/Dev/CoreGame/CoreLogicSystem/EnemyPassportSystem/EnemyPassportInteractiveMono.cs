using UnityEngine;
using UnityEngine.EventSystems;

namespace CyberNet.Core.EnemyPassport
{
    public class EnemyPassportInteractiveMono : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
    {
        private int _playerID;
        
        public void SetPlayerID(int playerID)
        {
            _playerID = playerID;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            EnemyPassportAction.SelectPlayerPassport?.Invoke(_playerID);
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            
            EnemyPassportAction.OnClickPlayerPassport?.Invoke(_playerID);
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            EnemyPassportAction.UnselectPlayerPassport?.Invoke(_playerID);
        }
    }
}