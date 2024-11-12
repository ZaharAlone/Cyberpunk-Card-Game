using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace CyberNet.Global.BlackScreen
{
    public class BlackScreenUIMono : MonoBehaviour
    {
        [SerializeField]
        [Required]
        private Image _imageBlackScreen;

        private Sequence _sequence;
        
        private const float time_animations = 0.25f;
        
        public void ForceShowScreen()
        {
            _imageBlackScreen.color = Color.black;
            _imageBlackScreen.gameObject.SetActive(true);
        }

        public void ForceHideScreen()
        {
            _imageBlackScreen.gameObject.SetActive(false);
        }
        
        public void AnimationsShowScreen()
        {
            _sequence = DOTween.Sequence();
            _sequence.Append(_imageBlackScreen.DOColor(Color.black, time_animations));
        }
        
        public void AnimationsHideScreen()
        {
            _sequence = DOTween.Sequence();
            _sequence.Append(_imageBlackScreen.DOColor(Color.clear, time_animations));
        }

        public void OnDestroy()
        {
            if (_sequence != null)
                _sequence.Kill();
        }
    }
}