using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using System.Collections.Generic;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.AI;
using CyberNet.Core.Battle.TacticsMode;
using CyberNet.Global;
using Random = UnityEngine.Random;

namespace CyberNet.Core.Battle
{
    [EcsSystem(typeof(CoreModule))]
    public class AISelectCardToPlayingInBattleSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        private const float modifier_efficiency = 0.1f;
        
        public void PreInit()
        {
            //BattleAction.SelectTacticsAI += SelectAICardInBattle;
        }
/*
        private SelectTacticsAndCardAIDTO SelectAICardInBattle(bool isAttackingPlayer)
        {
            var currentPlayerData = GetPlayerData(isAttackingPlayer);
            var enemyPlayerData = GetPlayerData(!isAttackingPlayer);
            
            var maxEnemyPower = CalculateEnemyMaxPower(enemyPlayerData);
            
            var cardPlayerPotentialTactics = CalculatePlayerCardsPotential(currentPlayerData.PlayerID);
            var selectCardAndTactics = SelectCardAndTacticsMaxEfficiency(cardPlayerPotentialTactics, maxEnemyPower);
            var selectTactics = new SelectTacticsAndCardAIDTO {
                GUIDCard = selectCardAndTactics.GUID, BattleTactics = selectCardAndTactics.SelectTactics.Key,
            };

            return selectTactics;
        }

        //Берем данные игрока из конфига
        private PlayerInBattleComponent GetPlayerData(bool isGetAttackingPlayer)
        {
            var currentBattleData = _dataWorld.OneData<BattleCurrentData>();

            if (isGetAttackingPlayer)
                return currentBattleData.AttackingPlayer;
            else
                return currentBattleData.DefendingPlayer;
        }

        //Считаем максимальную силу противника, исходя из его карт, учитывая погрешность, т.к. мы не знаем какие сейчас
        //карты на руке у игрока, + бот "не может помнить все", этим мы регулируем сложность боя
        private int CalculateEnemyMaxPower(PlayerInBattleComponent playerInBattle)
        {
            var maxPower = 0;
            var minPower = 100;

            var cardPotentialTactics = CalculatePlayerCardsPotential(playerInBattle.PlayerID);

            foreach (var cardPotential in cardPotentialTactics)
            {
                if (cardPotential.Power > maxPower)
                    maxPower = cardPotential.Power;

                if (cardPotential.Power < minPower)
                    minPower = cardPotential.Power;
            }

            var chanceErrorCalculate = 0;
            var botConfig = _dataWorld.OneData<BotConfigData>().BotConfigSO;

            if (playerInBattle.PlayerControlEntity == PlayerOrAI.AIEasy)
                chanceErrorCalculate = botConfig.MistakeInChoiceEasy;
            else if (playerInBattle.PlayerControlEntity == PlayerOrAI.AIMedium)
                chanceErrorCalculate = botConfig.MistakeInChoiceMedium;

            var randomMaxErrorCalculate = Random.Range(-chanceErrorCalculate, chanceErrorCalculate);

            maxPower += randomMaxErrorCalculate;

            var actualMaxPower = maxPower + playerInBattle.PowerPoint.BaseValue;
            var actualMinPower = minPower + playerInBattle.PowerPoint.BaseValue;

            if (actualMaxPower < actualMinPower)
                actualMaxPower = actualMinPower;
            
            return actualMaxPower;
        }

        //Составляем лист с потенциалом карт, чтобы на основе него в дальнейшем принимать решения
        private List<CardSelectTacticsPotential> CalculatePlayerCardsPotential(int playerID)
        {
            var battleTactics = _dataWorld.OneData<BattleTacticsData>().BattleTactics;
            var playerCardEntities = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.PlayerID == playerID)
                .Without<CardDiscardComponent>()
                .GetEntities();

            var cardPotentialTactics = new List<CardSelectTacticsPotential>();
            
            foreach (var cardEntity in playerCardEntities)
            {
                var cardComponent = cardEntity.GetComponent<CardComponent>();

                foreach (var selectTactics in battleTactics)
                {
                    var nextCardPotential = new CardSelectTacticsPotential {
                        GUID = cardComponent.GUID, SelectTactics = selectTactics,
                    };

                    nextCardPotential = CalculatePowerPotential(nextCardPotential, cardComponent.ValueLeftPoint, selectTactics.LeftCharacteristics);
                    nextCardPotential = CalculatePowerPotential(nextCardPotential, cardComponent.ValueRightPoint, selectTactics.RightCharacteristics);
                    nextCardPotential = CalculateAbilityCard(cardComponent, nextCardPotential);
                    
                    cardPotentialTactics.Add(nextCardPotential);
                }
            }
            
            return cardPotentialTactics;
        }

        //Считаем "силу" левого и правого значения карты поочередно
        private CardSelectTacticsPotential CalculatePowerPotential(CardSelectTacticsPotential tacticsPotential, int valueCard, BattleCharacteristics battleCharacteristics)
        {
            switch (battleCharacteristics)
            {
                case BattleCharacteristics.PowerPoint:
                    tacticsPotential.Power += valueCard;
                    break;
                case BattleCharacteristics.KillPoint:
                    tacticsPotential.Kill += valueCard;
                    break;
                case BattleCharacteristics.DefencePoint:
                    tacticsPotential.Defence += valueCard;
                    break;
            }
            return tacticsPotential;
        }
        
        //Считаем силу абилки карты
        private CardSelectTacticsPotential CalculateAbilityCard(CardComponent cardComponent, CardSelectTacticsPotential tacticsPotential)
        {
            if (cardComponent.Ability_1.AbilityType == AbilityType.None)
                return tacticsPotential;

            switch (cardComponent.Ability_1.AbilityType)
            {
                case AbilityType.PowerPoint:
                    tacticsPotential.Power += cardComponent.Ability_1.Count;
                    break;
                case AbilityType.KillPoint:
                    tacticsPotential.Kill += cardComponent.Ability_1.Count;
                    break;
                case AbilityType.DefencePoint:
                    tacticsPotential.Defence += cardComponent.Ability_1.Count;
                    break;
            }

            return tacticsPotential;
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
        */
        public void Destroy()
        {
          //  BattleAction.SelectTacticsAI -= SelectAICardInBattle;
        }
    }
}