using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace CyberNet.Core.ActionCard
{
    public class ActionCardEffectMono : MonoBehaviour
    {
        public TextMeshProUGUI Text;

        private Sequence _sequence;
        
        public void SetText(int value)
        {
            Text.text = value.ToString();
        }

        public void Init()
        {
            _sequence = DOTween.Sequence();
            _sequence.Append(transform.DOMoveY(25, 1));
        }

        public void OnDestroy()
        {
            if (_sequence != null)
                _sequence.Kill();
        }
    }
}