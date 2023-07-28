using CyberNet.Core.Ability;
using UnityEngine;

namespace CyberNet.Core.Ability
{
    public static class AbilityVisualEffect
    {
        public static void CreateEffect(CardAbilityEffectMono targetEffect, Vector3 position, int count = 0)
        {
            var effect = Object.Instantiate(targetEffect);
            effect.transform.position = position;
            if (count != 0)
                effect.SetText(count);
            effect.Init();
            Object.Destroy(effect.gameObject, 1);
        }
    }
}