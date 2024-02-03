using UnityEngine;
using UnityEngine.UI;

namespace CyberNet.Core.Arena.ArenaHUDUI
{
    public class ArenaContainerUICharacterMono : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _containerRectTransform;
        [SerializeField]
        private Image _characterAvatarImage;
        [SerializeField]
        private Image _frameAvatarImage;

        [Header("Size Avatar")]
        [SerializeField]
        private Vector2 _scaleBaseAvatar;
        [SerializeField]
        private Vector2 _scaleSelectAvatar;

        [HideInInspector]
        public int PositionInTurnQueue;
        
        public void SetVisualCharacter(Sprite spriteAvatar, Color32 frameColor)
        {
            _characterAvatarImage.sprite = spriteAvatar;
            _frameAvatarImage.color = frameColor;
        }

        public void OnCurrentCharacter()
        {
            _containerRectTransform.sizeDelta = _scaleSelectAvatar;
        }
        
        public void OffCurrentCharacter()
        {
            _containerRectTransform.sizeDelta = _scaleBaseAvatar;
        }

        public void KillCharacter()
        {
            
        }
    }
}