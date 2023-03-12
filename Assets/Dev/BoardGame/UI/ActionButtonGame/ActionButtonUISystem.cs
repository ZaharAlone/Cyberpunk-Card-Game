using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using ModulesFrameworkUnity;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace BoardGame.Core.UI
{
    [EcsSystem(typeof(BoardGameModule))]
    public class ActionButtonUISystem : IInitSystem, IRunSystem, IPostRunEventSystem<EventActionAttack>, IPostRunEventSystem<EventActionEndTurn>
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
                ui.UIMono.SetInteractiveButton(config.ActionPlayAll_loc, config.ActionPlayAll_image);
                actionPlayer.CurrentAction = ActionType.PlayAll;
            }
            else if (actionPlayer.TotalAttack - actionPlayer.SpendAttack != 0)
            {
                ui.UIMono.SetInteractiveButton(config.ActionAttack_loc, config.ActionAttack_image);
                actionPlayer.CurrentAction = ActionType.Attack;
            }
            else
            {
                ui.UIMono.SetInteractiveButton(config.ActionEndTurn_loc, config.ActionEndTurn_image);
                actionPlayer.CurrentAction = ActionType.EndTurn;
            }
        }

        public void PostRunEvent(EventActionAttack _) => Attack();
        public void PostRunEvent(EventActionEndTurn _) => EndTurn();

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
            ref var actionData = ref _dataWorld.OneData<ActionData>();
            var roundData = _dataWorld.OneData<RoundData>();
            var valueAttack = actionData.TotalAttack - actionData.SpendAttack;

            if (roundData.CurrentPlayer == PlayerEnum.Player)
            {
                ref var enemyStats = ref _dataWorld.OneData<EnemyStatsData>();
                enemyStats.Influence -= valueAttack;
            }
            else
            {
                ref var playerStats = ref _dataWorld.OneData<PlayerStatsData>();
                playerStats.Influence -= valueAttack;
            }

            actionData.SpendAttack += valueAttack;
            _dataWorld.RiseEvent(new EventUpdateBoardCard());
        }

        private async void EndTurn()
        {
            var cardInHand = _dataWorld.Select<CardComponent>().With<CardHandComponent>().GetEntities();

            foreach (var entity in cardInHand)
            {
                entity.RemoveComponent<CardHandComponent>();
                entity.AddComponent(new CardDiscardComponent());
            }

            var cardInDeck = _dataWorld.Select<CardComponent>().With<CardDeckComponent>().GetEntities();

            foreach (var entity in cardInDeck)
            {
                entity.RemoveComponent<CardDeckComponent>();
                entity.AddComponent(new CardDiscardComponent());
            }

            _dataWorld.RiseEvent(new EventUpdateBoardCard());
            _dataWorld.RiseEvent(new EventEndCurrentTurn());
        }
    }
}