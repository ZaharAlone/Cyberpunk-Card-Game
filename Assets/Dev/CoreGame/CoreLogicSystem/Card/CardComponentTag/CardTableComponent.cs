using UnityEngine.Serialization;
namespace CyberNet.Core
{
    public struct CardTableComponent
    {
        public SelectAbilityEnum SelectAbility;
    }

    public enum SelectAbilityEnum
    {
        Ability_1,
        Ability_2,
    }
}