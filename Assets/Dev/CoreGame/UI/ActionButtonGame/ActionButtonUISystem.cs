using CyberNet.Core.Sound;
using CyberNet.Global.Sound;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;

namespace CyberNet.Core.UI
{
    [EcsSystem(typeof(CoreModule))]
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
            var viewPlayer = _dataWorld.OneData<ViewPlayerData>();
            var ui = _dataWorld.OneData<UIData>();

            if (round.CurrentPlayer != viewPlayer.PlayerView)
            {
                ui.UIMono.HideInteractiveButton();
                return;
            }

            ui.UIMono.ShowInteractiveButton();
            var config = _dataWorld.OneData<BoardGameData>().BoardGameRule;
            ref var actionPlayer = ref _dataWorld.OneData<ActionData>();
            var cardInHand = _dataWorld.Select<CardComponent>()
                                       .Where<CardComponent>(card => card.Player == viewPlayer.PlayerView)
                                       .With<CardHandComponent>()
                                       .Count();

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

        public void PostRunEvent(EventActionAttack _)
        {
            Attack();
        }

        public void PostRunEvent(EventActionEndTurn _)
        {
            EndTurn();
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
            var entities = _dataWorld.Select<CardComponent>()
                                     .Where<CardComponent>(card => card.Player == PlayerEnum.Player1)
                                     .With<CardHandComponent>()
                                     .GetEntities();

            foreach (var entity in entities)
            {
                entity.RemoveComponent<CardHandComponent>();
                entity.AddComponent(new CardTableComponent());
            }

            _dataWorld.RiseEvent(new EventUpdateBoardCard());
        }

        private void Attack()
        {
            ref var actionData = ref _dataWorld.OneData<ActionData>();
            var roundData = _dataWorld.OneData<RoundData>();
            var valueAttack = actionData.TotalAttack - actionData.SpendAttack;

            if (roundData.CurrentPlayer == PlayerEnum.Player1)
            {
                ref var enemyStats = ref _dataWorld.OneData<Player2StatsData>();
                enemyStats.HP -= valueAttack;
            }
            else
            {
                ref var playerStats = ref _dataWorld.OneData<Player1StatsData>();
                playerStats.HP -= valueAttack;
            }

            ref var soundData = ref _dataWorld.OneData<SoundData>().Sound;
            SoundAction.PlaySound?.Invoke(soundData.AttackSound);
            actionData.SpendAttack += valueAttack;
            _dataWorld.RiseEvent(new EventUpdateBoardCard());
        }

        private void EndTurn()
        {
            var roundData = _dataWorld.OneData<RoundData>();

            var cardInHand = _dataWorld.Select<CardComponent>()
                                       .With<CardHandComponent>()
                                       .Where<CardComponent>(card => card.Player == roundData.CurrentPlayer)
                                       .GetEntities();

            foreach (var entity in cardInHand)
            {
                entity.RemoveComponent<CardHandComponent>();
                entity.AddComponent(new CardMoveToDiscardComponent());
            }

            var cardInDeck = _dataWorld.Select<CardComponent>().With<CardTableComponent>().GetEntities();

            foreach (var entity in cardInDeck)
            {
                entity.RemoveComponent<CardTableComponent>();
                entity.AddComponent(new CardMoveToDiscardComponent());
            }

            _dataWorld.RiseEvent(new EventUpdateBoardCard());
            var newEntity = _dataWorld.NewEntity();
            newEntity.AddComponent(new WaitEndRoundComponent());
        }
    }
}