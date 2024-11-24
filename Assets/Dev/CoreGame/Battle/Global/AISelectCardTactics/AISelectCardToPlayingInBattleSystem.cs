using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using System.Collections.Generic;
using CyberNet.Core.Battle.TacticsMode;

namespace CyberNet.Core.Battle
{
    [EcsSystem(typeof(CoreModule))]
    public class AISelectCardToPlayingInBattleSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        private const float modifier_efficiency = 0.1f;
        
        public void PreInit()
        {
            BattleAction.SelectTacticsAI += SelectAICardInBattle;
        }

        private void SelectAICardInBattle(int playerID)
        {
            var playerInBattleEntity = _dataWorld.Select<PlayerInBattleComponent>()
                .Where<PlayerInBattleComponent>(player => player.PlayerID == playerID)
                .SelectFirstEntity();
            
            var enemyPlayerID = _dataWorld.Select<PlayerInBattleComponent>()
                .Where<PlayerInBattleComponent>(player => player.PlayerID != playerID)
                .SelectFirst<PlayerInBattleComponent>().PlayerID;
            
            var maxEnemyPower = BattleAction.CalculatePlayerMaxPower.Invoke(enemyPlayerID);
            
            var cardPlayerPotentialTactics = BattleAction.CalculatePlayerCardsPotential?.Invoke(playerID);
            var selectCardAndTactics = SelectCardAndTacticsMaxEfficiency(cardPlayerPotentialTactics, maxEnemyPower);
            var selectTactics = new SelectTacticsAndCardComponent {
                GUIDCard = selectCardAndTactics.GUID,
                KeyCard = selectCardAndTactics.Key,
                BattleTacticsKey = selectCardAndTactics.SelectTactics.Key,
            };

            playerInBattleEntity.AddComponent(selectTactics);
        }

        private CardSelectTacticsPotential SelectCardAndTacticsMaxEfficiency(List<CardSelectTacticsPotential> cardPotentialTactics, int maxEnemyPower)
        {
            var nextListCardPotential = new List<CardSelectTacticsPotential>();
            var isVictoryTactics = false;
            
            foreach (var cardPotential in cardPotentialTactics)
            {
                var nextCardPotential = cardPotential;
                
                nextCardPotential.EfficiencyPower = (nextCardPotential.Power - maxEnemyPower) * modifier_efficiency + 1;
                nextCardPotential.EfficiencyKillDefence = (nextCardPotential.Kill + nextCardPotential.Defence) * modifier_efficiency;

                if (nextCardPotential.EfficiencyPower > 1f)
                    isVictoryTactics = true;
                
                nextListCardPotential.Add(nextCardPotential);
            }
            
            if (isVictoryTactics)
            {
                foreach (var cardPotential in nextListCardPotential)
                {
                    if (cardPotential.Power < 1f)
                        nextListCardPotential.Remove(cardPotential);
                }
            }
            
            nextListCardPotential.Sort(
                (x, y) => y.EfficiencyKillDefence.CompareTo(x.EfficiencyKillDefence));
            
            return nextListCardPotential[0];
        }
        
        public void Destroy()
        { 
            BattleAction.SelectTacticsAI -= SelectAICardInBattle;
        }
    }
}