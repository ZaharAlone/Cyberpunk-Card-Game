using CyberNet.Core.AbilityCard;
using CyberNet.Core.Battle.TacticsMode;

namespace CyberNet.Core.Battle
{
    public static class CalculatePlayerStatsInBattle
    {
        public static PowerKillDefenceDTO CalculateValueInMap(PlayerInBattleComponent playerStats)
        {
            var mapValue = new PowerKillDefenceDTO();
            
            mapValue.PowerPoint = playerStats.PowerPoint;
            mapValue.KillPoint = playerStats.KillPoint;
            mapValue.DefencePoint = playerStats.DefencePoint;

            return mapValue;
        }

        public static PowerKillDefenceDTO CalculateCardValue(PowerKillDefenceDTO valueStatsInBattle, BattleTactics battleTactics, CardConfigJson cardConfig)
        {
            valueStatsInBattle = CalculateValueCardAbility(valueStatsInBattle, battleTactics.LeftCharacteristics, cardConfig.ValueLeftPoint);
            valueStatsInBattle = CalculateValueCardAbility(valueStatsInBattle, battleTactics.RightCharacteristics, cardConfig.ValueRightPoint);
            valueStatsInBattle = CalculateAbilityCard(cardConfig, valueStatsInBattle);

            return valueStatsInBattle;
        }
        
        private static PowerKillDefenceDTO CalculateValueCardAbility(PowerKillDefenceDTO powerKillDefenceDTO, BattleCharacteristics battleChar, int cardPower)
        {
            switch (battleChar)
            {
                case BattleCharacteristics.PowerPoint:
                    powerKillDefenceDTO.PowerPoint += cardPower;
                    break;
                case BattleCharacteristics.KillPoint:
                    powerKillDefenceDTO.KillPoint += cardPower;
                    break;
                case BattleCharacteristics.DefencePoint:
                    powerKillDefenceDTO.DefencePoint += cardPower;
                    break;
            }
            return powerKillDefenceDTO;
        }
        
        private static PowerKillDefenceDTO CalculateAbilityCard(CardConfigJson cardConfig, PowerKillDefenceDTO playerValueInBattle)
        {
            if (cardConfig.Ability_1.AbilityType == AbilityType.None)
                return playerValueInBattle;
            
            switch (cardConfig.Ability_1.AbilityType)
            {
                case AbilityType.PowerPoint:
                    playerValueInBattle.PowerPoint += cardConfig.Ability_1.Count;
                    break;
                case AbilityType.KillPoint:
                    playerValueInBattle.KillPoint += cardConfig.Ability_1.Count;
                    break;
                case AbilityType.DefencePoint:
                    playerValueInBattle.DefencePoint += cardConfig.Ability_1.Count;
                    break;
            }

            return playerValueInBattle;
        }
    }
}