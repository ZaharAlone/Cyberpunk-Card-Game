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
            CalculateValueCardAction.TradeAction += TradeAction;
            CalculateValueCardAction.DrawCardAction += DrawCardAction;
            CalculateValueCardAction.DestroyCardAction += DestroyCardAction;
            CalculateValueCardAction.DiscardCardAction += DiscardCardAction;
            CalculateValueCardAction.NoiseCardAction += NoiseCardAction;
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