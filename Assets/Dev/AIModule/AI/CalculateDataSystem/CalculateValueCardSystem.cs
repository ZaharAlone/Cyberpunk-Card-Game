using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;

namespace CyberNet.Core.Enemy
{
    [EcsSystem(typeof(CoreModule))]
    public class CalculateValueCardSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            CalculateValueCardAction.AttackAction += AttackAction;
            CalculateValueCardAction.TradeAction += TradeAction;
            CalculateValueCardAction.InfluenceAction += InfluenceAction;
            CalculateValueCardAction.DrawCardAction += DrawCardAction;
            CalculateValueCardAction.DestroyCardAction += DestroyCardAction;
            CalculateValueCardAction.DiscardCardAction += DiscardCardAction;
            CalculateValueCardAction.NoiseCardAction += NoiseCardAction;
        }
        private int AttackAction(int count)
        {
            ref var hpPlayer1 = ref _dataWorld.OneData<Player1StatsData>().HP;
            ref var hpPlayer2 = ref _dataWorld.OneData<Player2StatsData>().HP;

            var calculateValue = (Mathf.Abs(hpPlayer1 - hpPlayer2) * 100 / 50) + count;
            return calculateValue;
        }
        private int TradeAction(int count)
        {
            //TODO: поправить
            return 15;
        }
        private int InfluenceAction(int count)
        {
            var hpBot = 0;
            var hpPlayer = 0;
            var playerRound = _dataWorld.OneData<RoundData>().CurrentPlayer;

            if (playerRound == PlayerEnum.Player1)
            {
                hpBot =  _dataWorld.OneData<Player1StatsData>().HP;
                hpPlayer =  _dataWorld.OneData<Player2StatsData>().HP;
            }
            else
            {
                hpPlayer =  _dataWorld.OneData<Player1StatsData>().HP;
                hpBot =  _dataWorld.OneData<Player2StatsData>().HP;
            }

            var value = (hpBot - hpPlayer) * 100 / 50;


            if (value < 0)
                value = Mathf.Abs(value) * 2;
            else if (hpBot - hpPlayer > 20)
                value *= 4;
            
            return value;
        }

        private int DrawCardAction()
        {
            return 50;
        }
        private int DestroyCardAction()
        {
            var playerRound = _dataWorld.OneData<RoundData>().CurrentPlayer;
            var countCardPlayer = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.Player == playerRound)
                .Count();
            var countCardPlayerNeutral = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.Player == playerRound && card.Nations == CardNations.Neutral)
                .Count();

            var value = 2 * (countCardPlayer + countCardPlayerNeutral);
            return value;
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