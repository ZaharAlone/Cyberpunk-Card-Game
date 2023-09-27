using CyberNet.Core.Sound;
using CyberNet.Global.Sound;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.WinLose;
using CyberNet.Global;
using CyberNet.Global.GameCamera;
using NotImplementedException = System.NotImplementedException;

namespace CyberNet.Core.UI
{
    /// <summary>
    /// Система управляющая Action кнопкой - позволяющей разыгрывать все карты, атаковать, заканчивать раунд
    /// </summary>
    [EcsSystem(typeof(CoreModule))]
    public class ActionPlayerButtonUISystem : IPreInitSystem, IInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            RoundAction.EndCurrentTurn += HideButton;
            RoundAction.StartTurn += UpdateButton;
            ActionPlayerButtonEvent.UpdateActionButton += UpdateButton;
            ActionPlayerButtonEvent.ClickActionButton += ClickButton;
            ActionPlayerButtonEvent.ActionEndTurnBot += EndTurn;
        }

        public void Init()
        {
            HideButton();
        }
        
        private void UpdateButton()
        {
            var roundData = _dataWorld.OneData<RoundData>();
            var ui = _dataWorld.OneData<CoreGameUIData>();
            
            if (roundData.playerTypeEnum != PlayerTypeEnum.Player || !roundData.EndPreparationRound)
            {
                ui.BoardGameUIMono.CoreHudUIMono.HideInteractiveButton();
                return;
            }

            ui.BoardGameUIMono.CoreHudUIMono.ShowInteractiveButton();
            var config = _dataWorld.OneData<BoardGameData>().BoardGameRule;
            ref var actionPlayer = ref _dataWorld.OneData<ActionCardData>();
            var cardInHand = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.PlayerID == roundData.CurrentPlayerID)
                .With<CardHandComponent>()
                .Count();

            ui.BoardGameUIMono.CoreHudUIMono.SetInteractiveButton(config.ActionEndTurn_loc, config.ActionEndTurn_image);
            actionPlayer.ActionPlayerButtonType = ActionPlayerButtonType.EndTurn;
            
            if (cardInHand > 0)
            {
                ui.BoardGameUIMono.CoreHudUIMono.SetInteractiveButton(config.ActionPlayAll_loc, config.ActionPlayAll_image);
                actionPlayer.ActionPlayerButtonType = ActionPlayerButtonType.PlayAll;
            }
            else
            {
                ui.BoardGameUIMono.CoreHudUIMono.SetInteractiveButton(config.ActionEndTurn_loc, config.ActionEndTurn_image);
                actionPlayer.ActionPlayerButtonType = ActionPlayerButtonType.EndTurn;
            }
        }
        
        private void HideButton()
        {
            var ui = _dataWorld.OneData<CoreGameUIData>();
            ui.BoardGameUIMono.CoreHudUIMono.HideInteractiveButton();
        }

        private void ClickButton()
        {
            ref var actionPlayer = ref _dataWorld.OneData<ActionCardData>();
            switch (actionPlayer.ActionPlayerButtonType)
            {
                case ActionPlayerButtonType.PlayAll:
                    PlayAll();
                    break;
                case ActionPlayerButtonType.EndTurn:
                    EndTurn();
                    break;
            }
        }

        private void PlayAll()
        {
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;
            var entities = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.PlayerID == currentPlayerID)
                .With<CardHandComponent>()
                .GetEntities();
            
            foreach (var entity in entities)
            {
                entity.RemoveComponent<CardHandComponent>();
                entity.AddComponent(new CardSelectAbilityComponent());
            }
            
            UpdateButton();
        }
        
        private void EndTurn()
        {
            var roundData = _dataWorld.OneData<RoundData>();

            var cardInHand = _dataWorld.Select<CardComponent>()
                                       .With<CardHandComponent>()
                                       .Where<CardComponent>(card => card.PlayerID == roundData.CurrentPlayerID)
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
            
            AnimationsMoveAtDiscardDeckAction.AnimationsMoveAtDiscardDeck?.Invoke();
            AbilityCardAction.ClearActionView.Invoke();
        }
    }
}