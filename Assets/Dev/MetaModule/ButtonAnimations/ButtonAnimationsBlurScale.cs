using DG.Tweening;
using UnityEngine;
namespace CyberNet.Meta
{
    public class ButtonAnimationsBlurScale : MonoBehaviour
    {
        [Header("Blur button on select")]
        [SerializeField]
        private bool _onBlur;
        [SerializeField]
        private GameObject _blurButton;

        [Header("Scale animations")]
        [SerializeField]
        private float _scaleSelectButton = 1.15f;
        [SerializeField]
        private float _scaleClickButton = 0.85f;

        private Sequence _sequence;
        public void AnimationsSelect()
        {
            _sequence = DOTween.Sequence();
            _sequence.Append(transform.DOScale(_scaleSelectButton, 0.15f));
            if (_onBlur)
                _blurButton.SetActive(true);
        }

        public void AnimationsDeselect()
        {
            _sequence = DOTween.Sequence();
            
            _sequence.Append(transform.DOScale(1f, 0.15f));
            if (_onBlur)
                _blurButton.SetActive(false);
        }
    }
}