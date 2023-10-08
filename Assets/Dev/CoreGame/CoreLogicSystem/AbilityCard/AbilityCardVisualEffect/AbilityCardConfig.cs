using System.Collections.Generic;
using UnityEngine;

namespace CyberNet.Core.AbilityCard
{
    [CreateAssetMenu(fileName = "CardAbilityEffect", menuName = "Scriptable Object/Board Game/Card Ability Effect")]
    public class AbilityCardConfig : ScriptableObject
    {
        [Header("Visual Effect")]
        public ActionCardEffectMono attackVFX;
        public ActionCardEffectMono tradeVFX;
        public ActionCardEffectMono influenceVFX;
        public ActionCardEffectMono discardActionCardVFX;
        
        [Header("UI Effect")]
        public GameObject DiscardCardUIEffect;

        [Header("Config")]
        public TextAsset ActionCardViewJsonConfig;
    }
}