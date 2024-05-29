using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CyberNet.Core.UI.ActionButtonAnimations
{
    public class ActionButtonScaleAnimationsMono : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private RectTransform _rectTransform;
        [Required]
        [SerializeField]
        private Image _imageIcons;
        
        [Header("Animations settings")]
        [SerializeField]
        private Vector3 _baseScale;
        [SerializeField]
        private Vector3 _bigScale;
        [SerializeField]
        private Vector3 _smallScale;
        
        [SerializeField]
        private Color32 _startColor;
        [SerializeField]
        private Color32 _endColor;
        
        [SerializeField]
        private float _durationAnimations = 0.3f;
        
        [SerializeField]
        private float _durationHideAnimations = 0.25f;
        
        private Sequence _sequence;
        
        public void HideIcons()
        {
            _sequence.Kill();
            _sequence = DOTween.Sequence();

            _sequence.Join(_imageIcons.DOColor(_endColor, _durationHideAnimations));
        }

        public void StartScaleAnimations()
        {
            _sequence.Kill();
            _sequence = DOTween.Sequence();
            _imageIcons.color = _startColor;
            
            _sequence.Append(_rectTransform.DOScale(_bigScale, _durationAnimations))
                .Append(_rectTransform.DOScale(_smallScale, _durationAnimations * 2))
                .Append(_rectTransform.DOScale(_baseScale, _durationAnimations))
                .SetLoops(-1, LoopType.Restart);
        }

        public void StopAnimations()
        {
            _sequence.Kill();
            _rectTransform.localScale = _baseScale;
        }

        private void OnDisable()
        {
            _sequence.Kill();
        }
    }
}