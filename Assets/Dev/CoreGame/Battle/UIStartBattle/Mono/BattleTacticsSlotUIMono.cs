using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace CyberNet.Core.Battle.TacticsMode
{
    public class BattleTacticsSlotUIMono : MonoBehaviour
    {
        private string _key;

        [SerializeField]
        [Required]
        private Image _leftImageCharacteristics;

        [SerializeField]
        [Required]
        private Image _rightImageCharacteristics;

        private Sequence _sequence;

        private const float time_animations = 0.2f;
        
        public void SetView(string key, Sprite leftCharacteristics, Sprite rightCharacteristics)
        {
            _key = key;
            _leftImageCharacteristics.sprite = leftCharacteristics;
            _rightImageCharacteristics.sprite = rightCharacteristics;
        }

        public void SwitchView(string key, Sprite leftCharacteristics, Sprite rightCharacteristics)
        {
            _key = key;
            
            if (_sequence != null)
                _sequence.Kill();

            _sequence = DOTween.Sequence();
            
            _sequence.Append(_leftImageCharacteristics.DOFade(0f, time_animations))
                .Join(_rightImageCharacteristics.DOFade(0f, time_animations))
                .OnComplete(() => FinishFadeCurrentCharacteristics(leftCharacteristics, rightCharacteristics));
        }

        private void FinishFadeCurrentCharacteristics(Sprite leftCharacteristics, Sprite rightCharacteristics)
        {
            _leftImageCharacteristics.sprite = leftCharacteristics;
            _rightImageCharacteristics.sprite = rightCharacteristics;
            _sequence.Append(_leftImageCharacteristics.DOFade(1f, time_animations))
                .Join(_rightImageCharacteristics.DOFade(1f, time_animations));
        }

        public void OnClickCurrentTactics()
        {
            BattleTacticsUIAction.SelectBattleTactics?.Invoke(_key);
        }

        public void OnDestroy()
        {
            if (_sequence != null)
                _sequence.Kill();
        }
    }
}