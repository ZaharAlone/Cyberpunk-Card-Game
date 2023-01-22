using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFrameworkUnity;
using System.Collections.Generic;
using UnityEngine;

namespace BoardGame.Core.UI
{
    [EcsSystem(typeof(BoardGameModule))]
    public class UIActionButtonSystem : IRunSystem
    {
        private DataWorld _dataWorld;

        public void Run()
        {
            var round = _dataWorld.OneData<RoundData>();
            var ui = _dataWorld.OneData<BoardGameUIComponent>();

            if (round.CurrentPlayer != PlayerEnum.Player)
            {
                ui.UIMono.HideInteractiveButton();
                return;
            }

            var config = _dataWorld.OneData<BoardGameData>().BoardGameRule;
            var actionPlayer = _dataWorld.OneData<ActionData>();
            var cardInHand = _dataWorld.Select<CardPlayerComponent>().With<CardInHandComponent>().Count();

            ui.UIMono.ShowInteractiveButton();

            if (cardInHand > 0)
            {
                ui.UIMono.SetInteractiveButton(config.ActionPlayAll);
            }
            else if (actionPlayer.TotalAttack - actionPlayer.SpendAttack != 0)
            {
                ui.UIMono.SetInteractiveButton(config.ActionAttack);
            }
            else
            {
                ui.UIMono.SetInteractiveButton(config.ActionEndTurn);
            }
        }

        private void PlayAll()
        {

        }

        private void Attack()
        {

        }

        private void EndTurn()
        {

        }
    }
}