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
    public class ActionButtonUISystem : IInitSystem, IRunSystem
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            ActionButtonEvent.ClickActionButton += ClickButton;
        }

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
            ref var actionPlayer = ref _dataWorld.OneData<ActionData>();
            var cardInHand = _dataWorld.Select<CardPlayerComponent>().With<CardHandComponent>().Count();

            ui.UIMono.ShowInteractiveButton();

            if (cardInHand > 0)
            {
                ui.UIMono.SetInteractiveButton(config.ActionPlayAll);
                actionPlayer.CurrentAction = ActionType.PlayAll;
            }
            else if (actionPlayer.TotalAttack - actionPlayer.SpendAttack != 0)
            {
                ui.UIMono.SetInteractiveButton(config.ActionAttack);
                actionPlayer.CurrentAction = ActionType.Attack;
            }
            else
            {
                ui.UIMono.SetInteractiveButton(config.ActionEndTurn);
                actionPlayer.CurrentAction = ActionType.EndTurn;
            }
        }

        private void ClickButton()
        {
            ref var actionPlayer = ref _dataWorld.OneData<ActionData>();
            switch (actionPlayer.CurrentAction)
            {
                case ActionType.PlayAll:
                    PlayAll();
                    break;
                case ActionType.Attack:
                    Attack();
                    break;
                case ActionType.EndTurn:
                    EndTurn();
                    break;
            }
        }

        private void PlayAll()
        {
            var entities = _dataWorld.Select<CardComponent>().With<CardPlayerComponent>().With<CardHandComponent>().GetEntities();

            foreach (var entity in entities)
            {
                entity.RemoveComponent<CardHandComponent>();
                entity.AddComponent(new CardDeckComponent());
            }

            _dataWorld.RiseEvent(new EventUpdateBoardCard());
        }

        private void Attack()
        {
            ref var actionPlayer = ref _dataWorld.OneData<ActionData>();
            ref var enemyStats = ref _dataWorld.OneData<EnemyStatsData>();
            var valueAttack = actionPlayer.TotalAttack - actionPlayer.SpendAttack;

            enemyStats.Influence -= valueAttack;
            actionPlayer.SpendAttack += valueAttack;
            _dataWorld.RiseEvent(new EventUpdateBoardCard());
        }

        private void EndTurn()
        {
            var cardInHand = _dataWorld.Select<CardComponent>().With<CardPlayerComponent>().With<CardHandComponent>().GetEntities();

            foreach (var entity in cardInHand)
            {
                entity.RemoveComponent<CardHandComponent>();
                entity.AddComponent(new CardDiscardComponent());
            }

            var cardInDeck = _dataWorld.Select<CardComponent>().With<CardPlayerComponent>().With<CardDeckComponent>().GetEntities();

            foreach (var entity in cardInDeck)
            {
                entity.RemoveComponent<CardDeckComponent>();
                entity.AddComponent(new CardDiscardComponent());
            }

            _dataWorld.RiseEvent(new EventEndCurrentTurn());
        }
    }
}