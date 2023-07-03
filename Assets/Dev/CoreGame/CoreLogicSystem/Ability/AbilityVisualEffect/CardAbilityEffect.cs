using UnityEngine;

namespace CyberNet.Core.Ability
{
    [CreateAssetMenu(fileName = "CardAbilityEffect", menuName = "Scriptable Object/Board Game/Card Ability Effect")]
    public class CardAbilityEffect : ScriptableObject
    {
        public CardAbilityEffectMono AttackAbilityVFX;
        public CardAbilityEffectMono TradeAbilityVFX;
        public CardAbilityEffectMono InfluenceAbilityVFX;
    }
}