using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CyberNet.Core.UI
{
    public class CharacterDamagePassportEffect : MonoBehaviour
    {
        public ParticleSystem AttackEffect;
        public Image ShieldVFX;

        private Sequence _sequence;

        public void Awake()
        {
            var material = ShieldVFX.material;
            ShieldVFX.material = new Material(material);
        }
        
        public void Attack()
        {
            AttackEffect.Play();
            ShieldVFX.material.SetFloat("_MaskPower", 2f);
            _sequence = DOTween.Sequence();
            _sequence.Append(ShieldVFX.material.DOFloat(10f, "_MaskPower", 2f));
        }
    }   
}