using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using System.Collections.Generic;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.AI;
using CyberNet.Global;
using Random = UnityEngine.Random;

namespace CyberNet.Core.Battle.TacticsMode
{
    [EcsSystem(typeof(CoreModule))]
    public class CalculateEnemyMaxPowerSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            BattleAction.CalculatePlayerMaxPower += CalculatePlayerMaxPower;
            BattleAction.CalculatePlayerCardsPotential += CalculatePlayerCardsPotential;
        }
        
         //Считаем максимальную силу противника, исходя из его карт, учитывая погрешность, т.к. мы не знаем какие сейчас
        //карты на руке у игрока, + бот "не может помнить все", этим мы регулируем сложность боя
        private int CalculatePlayerMaxPower(int playerID)
        {
            var playerInBattleComponent = _dataWorld.Select<PlayerInBattleComponent>()
                .Where<PlayerInBattleComponent>(player => player.PlayerID == playerID)
                .SelectFirst<PlayerInBattleComponent>();
            
            var maxPower = 0;
            var minPower = 100;

            var cardPotentialTactics = CalculatePlayerCardsPotential(playerID);

            foreach (var cardPotential in cardPotentialTactics)
            {
                if (cardPotential.Power > maxPower)
                    maxPower = cardPotential.Power;

                if (cardPotential.Power < minPower)
                    minPower = cardPotential.Power;
            }

            var chanceErrorCalculate = 0;
            var botConfig = _dataWorld.OneData<BotConfigData>().BotConfigSO;

            if (playerInBattleComponent.PlayerControlEntity == PlayerOrAI.AIEasy)
                chanceErrorCalculate = botConfig.MistakeInChoiceEasy;
            else if (playerInBattleComponent.PlayerControlEntity == PlayerOrAI.AIMedium)
                chanceErrorCalculate = botConfig.MistakeInChoiceMedium;

            var randomMaxErrorCalculate = Random.Range(-chanceErrorCalculate, chanceErrorCalculate);

            maxPower += randomMaxErrorCalculate;

            var actualMaxPower = maxPower + playerInBattleComponent.PowerPoint;
            var actualMinPower = minPower + playerInBattleComponent.PowerPoint;

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

        public void Destroy()
        {
            BattleAction.CalculatePlayerMaxPower -= CalculatePlayerMaxPower;
            BattleAction.CalculatePlayerCardsPotential -= CalculatePlayerCardsPotential;
        }
    }
}