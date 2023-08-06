using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace CyberNet.Core.ActionCard
{
    public class ActionCardEffectMono : MonoBehaviour
    {
        public RectTransform ParticleTranform;
        public TextMeshProUGUI Text;

        private Sequence _sequence;
        
        public void SetText(int value)
        {
            if (value > 1)
            {
                Text.gameObject.SetActive(true);
                ParticleTranform.anchoredPosition = new Vector2(-20, 0);
            }
            else
            {
                Text.gameObject.SetActive(false);
                ParticleTranform.anchoredPosition = Vector2.zero;
            }
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