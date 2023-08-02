using CyberNet.Core.ActionCard;
using UnityEngine;

namespace CyberNet.Core.ActionCard
{
    public static class ActionCardVisualEffect
    {
        public static void CreateEffect(ActionCardEffectMono targetEffect, Vector3 position, int count = 0)
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