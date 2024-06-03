using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.EndTurnWarningPopup;
using CyberNet.Global;
using DG.Tweening;
using CyberNet.Core.UI.ActionButton;
using UnityEngine;

namespace CyberNet.Core.UI
{
    /// <summary>
    /// Система управляющая Action кнопкой - позволяющей разыгрывать все карты, атаковать, заканчивать раунд
    /// </summary>
    [EcsSystem(typeof(CoreModule))]
    public class ActionPlayerButtonUISystem : IPreInitSystem, IInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        private const string scout_card_name = "neutral_scout";

        public void PreInit()
        {
            RoundAction.EndCurrentTurn += HideButton;
            RoundAction.StartTurn += UpdateButton;
            
            ActionPlayerButtonEvent.UpdateActionButton += UpdateButton;
            ActionPlayerButtonEvent.ClickActionButton += ClickActionButton;
            ActionPlayerButtonEvent.ForceEndRound += EndTurn;
            ActionPlayerButtonEvent.ActionEndTurnBot += EndTurn;
        }

        public void Init()
        {
            ForceHideActionButton();
        }

        private void ForceHideActionButton()
        {
            var actionButton = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CoreHudUIMono.CoreActionButtonAnimationsMono;
            actionButton.ForceHideActionButton();
        }
        
        private void UpdateButton()
        {
            var roundData = _dataWorld.OneData<RoundData>();
            var ui = _dataWorld.OneData<CoreGameUIData>();
            var coreActionButton = ui.BoardGameUIMono.CoreHudUIMono.CoreActionButtonAnimationsMono;
            
            if (roundData.playerOrAI != PlayerOrAI.Player && coreActionButton.IsEnableButton)//??? PauseInteractive???   || roundData.PauseInteractive
            {
                coreActionButton.HideButtonPlayAnimations();
            }
            else if (roundData.playerOrAI == PlayerOrAI.Player)
            {
                if (!coreActionButton.IsEnableButton)
                    coreActionButton.ShowButtonPlayAnimations();
                
                UpdateViewEnableButton();
            }
        }

        private void UpdateViewEnableButton()
        {
            var roundData = _dataWorld.OneData<RoundData>();
            var ui = _dataWorld.OneData<CoreGameUIData>();
            var boardGameRule = _dataWorld.OneData<BoardGameData>().BoardGameRule;
            
            var cardInHandCount = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.PlayerID == roundData.CurrentPlayerID)
                .With<CardHandComponent>()
                .Count();

            ref var actionPlayer = ref _dataWorld.OneData<ActionCardData>();
            var isOnlyCardScout = CheckOnlyCardScoutInHandPlayer();
            var isFreeCardInTradeRow = _dataWorld.Select<CardComponent>()
                .With<CardTradeRowComponent>()
                .With<CardFreeToBuyComponent>()
                .Count() > 0;

            var cardIsHand = cardInHandCount > 0;
            var playAllCard = isOnlyCardScout && cardIsHand;
            var endRoundNoActive = isFreeCardInTradeRow || cardIsHand;
            
            var coreHUDMono = ui.BoardGameUIMono.CoreHudUIMono;
            
            if (playAllCard)
            {
                actionPlayer.ActionPlayerButtonType = ActionPlayerButtonType.PlayAll;
                
                coreHUDMono.CoreActionButtonAnimationsMono.SetStateViewButton(ActionPlayerButtonType.PlayAll);
                coreHUDMono.CoreActionButtonAnimationsMono.SetAnimationsReadyClick();
                coreHUDMono.PopupActionButton.SetKeyPopup(boardGameRule.PlayAllPopup);
            }
            else
            {
                actionPlayer.ActionPlayerButtonType = ActionPlayerButtonType.EndTurn;
                coreHUDMono.CoreActionButtonAnimationsMono.SetStateViewButton(ActionPlayerButtonType.EndTurn);
                coreHUDMono.PopupActionButton.SetKeyPopup(boardGameRule.EndRoundPopup);

                if (endRoundNoActive)
                {
                    coreHUDMono.CoreActionButtonAnimationsMono.SetAnimationsNotReadyButtonClick();
                }
                else
                {
                    coreHUDMono.CoreActionButtonAnimationsMono.SetAnimationsReadyClick();
                }
            }
        }

        private bool CheckOnlyCardScoutInHandPlayer()
        {
            var isOnlyScoutCard = true;
            
            var roundData = _dataWorld.OneData<RoundData>();
            
            var cardHandEntities = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.PlayerID == roundData.CurrentPlayerID)
                .With<CardHandComponent>()
                .GetEntities();

            foreach (var cardHandEntity in cardHandEntities)
            {
                var cardComponent = cardHandEntity.GetComponent<CardComponent>();
                if (cardComponent.Key != scout_card_name)
                {
                    isOnlyScoutCard = false;
                    break;
                }
            }
            
            return isOnlyScoutCard;
        }
        
        private void HideButton()
        {
            var ui = _dataWorld.OneData<CoreGameUIData>();
            ui.BoardGameUIMono.CoreHudUIMono.CoreActionButtonAnimationsMono.HideButtonPlayAnimations();
        }

        private void ClickActionButton()
        {
            ref var actionPlayer = ref _dataWorld.OneData<ActionCardData>();

            if (actionPlayer.ActionPlayerButtonType == ActionPlayerButtonType.PlayAll)
            {
                PlayAll();
            }
            else if (actionPlayer.ActionPlayerButtonType == ActionPlayerButtonType.EndTurn)
            {
                var isOpenWarningPopup = СheckingPlayerHasAnyActionsLeft();
                
                if (!isOpenWarningPopup)
                    EndTurn();
            }
        }

        private bool СheckingPlayerHasAnyActionsLeft()
        {
            var isActionPlayerLeft = false;
            
            var roundData = _dataWorld.OneData<RoundData>();
            var cardInHandCount = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.PlayerID == roundData.CurrentPlayerID)
                .With<CardHandComponent>()
                .Count();

            if (cardInHandCount > 0)
            {
                EndTurnWarningPopupAction.OpenPopupUnuseCard?.Invoke();
                return true;
            }
                
            var isFreeCardInTradeRow = _dataWorld.Select<CardComponent>()
                .With<CardTradeRowComponent>()
                .With<CardFreeToBuyComponent>()
                .Count() > 0;

            if (isFreeCardInTradeRow)
            {
                EndTurnWarningPopupAction.OpenPopupUnuseMoney?.Invoke();
                return true;
            }

            return false;
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
                entity.AddComponent(new CardAbilitySelectionCompletedComponent
                { 
                    SelectAbility = SelectAbilityEnum.Ability_0,
                    OneAbilityInCard = true
                });

                entity.RemoveComponent<CardHandComponent>();
                if (entity.HasComponent<CardComponentAnimations>())
                {
                    var animationCard = entity.GetComponent<CardComponentAnimations>();
                    animationCard.Sequence.Kill();
                    entity.RemoveComponent<CardComponentAnimations>();
                }
                
                entity.AddComponent(new CardStartMoveToTableComponent());
                entity.AddComponent(new CardPlayAllComponent());
            }
            
            AnimationsMoveBoardCardAction.AnimationsMoveBoardCard?.Invoke();
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

            var cardInDeck = _dataWorld.Select<CardComponent>().With<CardAbilitySelectionCompletedComponent>().GetEntities();

            foreach (var entity in cardInDeck)
            {
                entity.RemoveComponent<CardAbilitySelectionCompletedComponent>();
                entity.RemoveComponent<CardInTableComponent>();
                entity.AddComponent(new CardMoveToDiscardComponent());
            }

            var cardCanUse = _dataWorld.Select<CardCanUseComponent>().GetEntities();

            foreach (var cardEntity in cardCanUse)
            {
                cardEntity.RemoveComponent<CardCanUseComponent>();
            }
            
            _dataWorld.NewEntity().AddComponent(new WaitEndRoundComponent());
            
            AnimationsMoveAtDiscardDeckAction.AnimationsMoveAtDiscardDeck?.Invoke();
            AbilityCardAction.ClearActionView.Invoke();
        }

        public void Destroy()
        {
            RoundAction.EndCurrentTurn -= HideButton;
            RoundAction.StartTurn -= UpdateButton;
            ActionPlayerButtonEvent.UpdateActionButton -= UpdateButton;
            ActionPlayerButtonEvent.ClickActionButton -= ClickActionButton;
            ActionPlayerButtonEvent.ForceEndRound -= EndTurn;
            ActionPlayerButtonEvent.ActionEndTurnBot -= EndTurn;
        }
    }
}