using UnityEngine;
using UnityEngine.Serialization;

namespace CyberNet.Core.ActionCard
{
    [CreateAssetMenu(fileName = "CardAbilityEffect", menuName = "Scriptable Object/Board Game/Card Ability Effect")]
    public class ActionCardConfig : ScriptableObject
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