using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
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
            CalculateValueCardAction.AttackAction += AttackAction;
            CalculateValueCardAction.MoveUnitAction += MoveUnitAction;
            CalculateValueCardAction.TradeAction += TradeAction;
            CalculateValueCardAction.DrawCardAction += DrawCardAction;
            CalculateValueCardAction.DestroyCardAction += DestroyCardAction;
            CalculateValueCardAction.DiscardCardAction += DiscardCardAction;
            CalculateValueCardAction.NoiseCardAction += NoiseCardAction;
        }

        private int AttackAction(int count)
        {
            //TODO: поправить
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

            var result = countUnitInMap * 5;
            
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