namespace CyberNet.Core
{
    public struct CardAbilitySelectionCompletedComponent
    {
        public SelectAbilityEnum SelectAbility;

        public bool CalculateBaseAbility;
        public bool CalculateComboAbility;

        public bool OneAbilityInCard;
    }

    public enum SelectAbilityEnum
    {
        Ability_0,
        Ability_1,
    }
}