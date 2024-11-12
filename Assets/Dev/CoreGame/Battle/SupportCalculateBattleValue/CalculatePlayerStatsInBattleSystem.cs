using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using System.Collections.Generic;
using CyberNet.Core.Battle;
using CyberNet.Core.Battle.TacticsMode;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class CalculatePlayerStatsInBattleSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            BattleAction.CalculatePlayerStatsInBattle += CalculatePlayerStats;
        }

        private PowerKillDefenceDTO CalculatePlayerStats(PowerKillDefenceDTO valueStatsInBattle, Entity playerEntity)
        {
            if (!playerEntity.HasComponent<SelectTacticsAndCardComponent>())
                return valueStatsInBattle;
            
            var battleTactics = _dataWorld.OneData<BattleTacticsData>().BattleTactics;
            var cardsConfig = _dataWorld.OneData<CardsConfigData>().Cards;
            
            var selectCardInTacticsComponent = playerEntity.GetComponent<SelectTacticsAndCardComponent>();
            var currentTacticsIndex = SelectCurrentTacticsIndex(selectCardInTacticsComponent.BattleTacticsKey, battleTactics);
            var targetTactics = battleTactics[currentTacticsIndex];
            var configCurrentCard = cardsConfig[selectCardInTacticsComponent.KeyCard];

            valueStatsInBattle = CalculatePlayerStatsInBattle.CalculateCardValue(valueStatsInBattle, targetTactics, configCurrentCard);
            return valueStatsInBattle;
        }
        
        private int SelectCurrentTacticsIndex(string currentTacticsKey, List<BattleTactics> battleTacticsList)
        {
            var listTactics = new List<string>();
            foreach (var currentTactics in battleTacticsList)
                listTactics.Add(currentTactics.Key);
            
            var targetIndex = listTactics.IndexOf(currentTacticsKey);
            return targetIndex;
        }

        public void Destroy()
        {
            BattleAction.CalculatePlayerStatsInBattle -= CalculatePlayerStats;
        }
    }
}