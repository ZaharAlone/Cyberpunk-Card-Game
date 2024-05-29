using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CyberNet.Core.UI.ActionButtonAnimations
{
    public class ActionButtonMoveAnimationsMono : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private RectTransform _rectTransform;
        [Required]
        [SerializeField]
        private Image _imageIcons;

        [Header("Animations settings")]
        [SerializeField]
        private Vector2 _basePositions;
        [SerializeField]
        private Vector2 _startAnimationsPositions;
        [SerializeField]
        private Vector2 _endAnimationsPositions;
        [SerializeField]
        private Color32 _startColor;
        [SerializeField]
        private Color32 _endColor;
        [SerializeField]
        private float _durationAnimations = 0.3f;
        
        [SerializeField]
        private float _durationHideAnimations = 0.25f;
        
        private Sequence _sequence;

        private void OnEnable()
        {
            _rectTransform.localPosition = _basePositions;
            _imageIcons.color = _startColor;
        }

        public void HideIcons()
        {
            _sequence.Kill();
            _sequence = DOTween.Sequence();

            _sequence.Join(_imageIcons.DOColor(_endColor, _durationHideAnimations));
        }

        public void StartMoveAnimations()
        {
            _sequence.Kill();
            _sequence = DOTween.Sequence();
            
            _sequence.Append(_rectTransform.DOAnchorPos(_endAnimationsPositions, _durationAnimations))
                .Join(_imageIcons.DOColor(_endColor, _durationAnimations))
                .Append(_rectTransform.DOAnchorPos(_startAnimationsPositions, 0.01f))
                .Join(_imageIcons.DOColor(_startColor, 0.01f))
                .SetLoops(-1, LoopType.Restart);
        }

        public void StopAnimations()
        {
            _sequence.Kill();
            _rectTransform.localPosition = _basePositions;
            _imageIcons.color = _startColor;
        }

        private void OnDisable()
        {
            _sequence.Kill();
        }
    }
}