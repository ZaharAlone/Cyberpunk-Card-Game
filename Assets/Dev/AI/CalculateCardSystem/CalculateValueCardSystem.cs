using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.City;
using CyberNet.Core.Player;

namespace CyberNet.Core.AI
{
    [EcsSystem(typeof(CoreModule))]
    public class CalculateValueCardSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            CalculateValueCardAction.CalculateValueCardAbility += CalculateValueCardAbility;
        }
        
        private int CalculateValueCardAbility(AbilityCardContainer abilityCard)
        {
            var value = 0;
            
            switch (abilityCard.AbilityType)
            {
                case AbilityType.Attack:
                    value = AttackAction(abilityCard.Count);
                    break;
                case AbilityType.Trade:
                    value = TradeAction(abilityCard.Count);
                    break;
                case AbilityType.DrawCard:
                    break;
                case AbilityType.DestroyCard:
                    value = DestroyCardAction();
                    break;
                case AbilityType.SquadMove:
                    value = MoveUnitAction();
                    break;
            }

            return value;
        }

        private int AttackAction(int count)
        {
            //TODO: поправить значение, логика верная
            return 15 * count;
        }

        private int MoveUnitAction()
        {
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();
            var playerComponent = playerEntity.GetComponent<PlayerComponent>();

            var countUnitInMap = _dataWorld.Select<UnitMapComponent>()
                .Where<UnitMapComponent>(unit => unit.PowerSolidPlayerID == playerComponent.PlayerID)
                .Count();

            var result = countUnitInMap * 6;

            if (result <= 12)
                return result;

            var potentialAttack = AbilityAIAction.CalculatePotentialMoveUnitAttack.Invoke();
            if (potentialAttack.Value > 0)
                return 20;

            var potentialMoveMyTower = AbilityAIAction.CalculatePotentialMoveUnit.Invoke();

            result = 0;
            if (potentialMoveMyTower.Value != 0)
            {
                result = 14;
            }
            return result;
        }

        private int TradeAction(int count)
        {
            //TODO: поправить
            return 15;
        }

        private int DrawCardAction()
        {
            return 50;
        }
        private int DestroyCardAction()
        {
            //TODO вернуть
            /*
            var playerRound = _dataWorld.OneData<RoundData>().CurrentPlayer;
            var countCardPlayer = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.Player == playerRound)
                .Count();
            var countCardPlayerNeutral = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.Player == playerRound && card.Nations == CardNations.Neutral)
                .Count();

            var value = 2 * (countCardPlayer + countCardPlayerNeutral);*/
            return 0;//value
        }
        private int DiscardCardAction()
        {
            //const
            return 40;
        }
        private int NoiseCardAction()
        {
            //const
            return 40;
        }
    }
}