using Sirenix.OdinInspector;
using UnityEngine;

namespace CyberNet.Core.Arena.ArenaHUDUI
{
    public class ArenaHUDUIMono : MonoBehaviour
    {
        [SerializeField]
        private Transform _containerListCharacter;

        public void OnArenaHUD()
        {
            _containerListCharacter.gameObject.SetActive(true);
        }
        
        public void OffArenaHUD()
        {
            _containerListCharacter.gameObject.SetActive(false);
        }

        [Button]
        public void NextRoundTest()
        {
            NextRoundUpdateViewAvatar();
        }
        
        public void NextRoundUpdateViewAvatar()
        {
            _containerListCharacter.GetChild(0).SetAsLastSibling();
            
        }
    }
}