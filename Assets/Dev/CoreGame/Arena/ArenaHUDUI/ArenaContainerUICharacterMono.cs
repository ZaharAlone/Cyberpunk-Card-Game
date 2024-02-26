using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CyberNet.Core.Arena.ArenaHUDUI
{
    public class ArenaContainerUICharacterMono : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _avatarRectTransform;
        
        [HideInInspector]
        public int PositionInTurnQueue;

        [SerializeField]
        private Image _characterAvatarImage;
        [SerializeField]
        private Image _frameAvatarImage;

        private Color32 _frameColor;
        private Sequence _sequence;

        public void SetVisualCharacter(Sprite spriteAvatar, Color32 frameColor)
        {
            _characterAvatarImage.sprite = spriteAvatar;
            _frameAvatarImage.color = frameColor;
            _frameColor = frameColor;
        }

        public void SetPositionAndScale(Vector2 position, float scale)
        {
            _avatarRectTransform.anchoredPosition = position;
            _avatarRectTransform.sizeDelta = Vector2.one * scale;
        }

        public void AnimationsMoveNext(float position, float scale)
        {
            _sequence = DOTween.Sequence();
            _sequence.Append(_avatarRectTransform.DOAnchorPos(new Vector2(position, 0), 0.5f))
                .Join(_avatarRectTransform.DOSizeDelta(new Vector2(scale, scale), 0.5f));
        }
        
        public void AnimationsMoveBack(float position, float scale)
        {
            _characterAvatarImage.color = new Color32(255, 255, 255, 0);
            var transparentFrameColor = _frameColor;
            transparentFrameColor.a = 0;
            _frameAvatarImage.color = transparentFrameColor;
            _avatarRectTransform.anchoredPosition = new Vector2(position, 0);
            _avatarRectTransform.sizeDelta = new Vector2(scale - 20, scale - 20);
            
            _sequence = DOTween.Sequence();
            _sequence.PrependInterval(0.25f)
                .Append(_avatarRectTransform.DOSizeDelta(new Vector2(scale, scale), 0.5f))
                .Join(_characterAvatarImage.DOColor(Color.white, 0.5f))
                .Join(_frameAvatarImage.DOColor(_frameColor, 0.5f));
        }

        public void KillCharacter()
        {
            
        }
    }
}