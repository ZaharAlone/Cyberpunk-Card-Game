using CyberNet.Core.AbilityCard;
using UnityEngine;

namespace CyberNet.Core.AbilityCard
{
    public static class ActionVisualEffect
    {
        public static void CreateEffect(ActionCardEffectMono targetEffect, Vector3 position, Transform parent, int count = 0)
        {
            var effect = Object.Instantiate(targetEffect, parent);
            effect.transform.position = position;
            if (count != 0)
                effect.SetText(count);
            effect.Init();
            Object.Destroy(effect.gameObject, 1);
        }
    }
}