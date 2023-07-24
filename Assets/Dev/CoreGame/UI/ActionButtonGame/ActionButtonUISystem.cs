using CyberNet.Core.Sound;
using CyberNet.Global.Sound;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.Ability;

namespace CyberNet.Core.UI
{
    /// <summary>
    /// Система управляющая Action кнопкой - позволяющей разыгрывать все карты, атаковать, заканчивать раунд
    /// </summary>
    [EcsSystem(typeof(CoreModule))]
    public class ActionButtonUISystem : IInitSystem, IRunSystem
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            ActionButtonEvent.ClickActionButton += ClickButton;
            ActionButtonEvent.ActionAttackBot += Attack;
            ActionButtonEvent.ActionEndTurnBot += EndTurn;
        }

        //TO-DO перевести на ивент
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
            ref var actionPlayer = ref _dataWorld.OneData<AbilityData>();
            var cardInHand = _dataWorld.Select<CardComponent>()
                                       .Where<CardComponent>(card => card.Player == viewPlayer.PlayerView)
                                       .With<CardHandComponent>()
                                       .Count();

            if (cardInHand > 0)
            {
                ui.UIMono.SetInteractiveButton(config.ActionPlayAll_loc, config.ActionPlayAll_image);
                actionPlayer.ActionType = ActionType.PlayAll;
            }
            else if (actionPlayer.TotalAttack - actionPlayer.SpendAttack != 0)
            {
                ui.UIMono.SetInteractiveButton(config.ActionAttack_loc, config.ActionAttack_image);
                actionPlayer.ActionType = ActionType.Attack;
            }
            else
            {
                ui.UIMono.SetInteractiveButton(config.ActionEndTurn_loc, config.ActionEndTurn_image);
                actionPlayer.ActionType = ActionType.EndTurn;
            }
        }
        
        private void ClickButton()
        {
            ref var actionPlayer = ref _dataWorld.OneData<AbilityData>();
            switch (actionPlayer.ActionType)
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
                entity.AddComponent(new CardSelectAbilityComponent());
            }
        }

        private void Attack()
        {
            ref var boardGameRule = ref _dataWorld.OneData<BoardGameData>().BoardGameRule;
            ref var actionData = ref _dataWorld.OneData<AbilityData>();
            var roundData = _dataWorld.OneData<RoundData>();
            var valueAttack = actionData.TotalAttack - actionData.SpendAttack;
            var percentHP = 0f;
            
            if (roundData.CurrentPlayer == PlayerEnum.Player1)
            {
                ref var player2Stats = ref _dataWorld.OneData<Player2StatsData>();
                player2Stats.HP -= valueAttack;
                percentHP = (float)player2Stats.HP / boardGameRule.BaseInfluenceCount;
            }
            else
            {
                ref var playerStats = ref _dataWorld.OneData<Player1StatsData>();
                playerStats.HP -= valueAttack;
                percentHP = (float)playerStats.HP / boardGameRule.BaseInfluenceCount;
            }
            
            AttackView(roundData.CurrentPlayer, valueAttack, percentHP);

            ref var soundData = ref _dataWorld.OneData<SoundData>().Sound;
            SoundAction.PlaySound?.Invoke(soundData.AttackSound);
            actionData.SpendAttack += valueAttack;
            
            AbilityEvent.UpdateValueResourcePlayedCard?.Invoke();
        }

        private void AttackView(PlayerEnum targetAttack, int valueAttack, float percentHP)
        {
            ref var boardUI = ref _dataWorld.OneData<UIData>().UIMono;
            var viewData = _dataWorld.OneData<ViewPlayerData>();
            if (targetAttack != viewData.PlayerView)
            {
                boardUI.characterDamagePassportEffectDown.Attack();
                boardUI.DamageScreen.Damage(valueAttack, percentHP);
            }
            else
                boardUI.characterDamagePassportEffectUp.Attack();
            
            BoardGameCameraEvent.GetDamageCameraShake?.Invoke();
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
            AbilityEvent.ClearActionView.Invoke();
        }
    }
}