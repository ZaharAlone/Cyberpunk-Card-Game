using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CyberNet.Core.UI
{
    public class CharacterDamageScreenEffect : MonoBehaviour
    {
        public Image DamageScreen;

        private Sequence _sequence;

        public void Awake()
        {
            var material = DamageScreen.material;
            DamageScreen.material = new Material(material);
        }

        public void Damage(int damage, float percentHP)
        {
            var finalValueShield = Mathf.Lerp(8f, 20f, percentHP);
            _sequence = DOTween.Sequence();
            _sequence.Append(DamageScreen.material.DOFloat(8, "_MaskPower", 0.2f))
                     .Append(DamageScreen.material.DOFloat(finalValueShield, "_MaskPower", 0.75f));
        }
    }
}