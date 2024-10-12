using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.EndTurnWarningPopup;
using CyberNet.Core.UI;
using CyberNet.Core.UI.ActionButton;
using DG.Tweening;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class ActionPlayerButtonUILogicSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            ActionPlayerButtonEvent.ClickActionButton += ClickActionButton;
            ActionPlayerButtonEvent.CheckPlayerHasAnyActionsLeft += СheckingPlayerHasAnyActionsLeft;
            ActionPlayerButtonEvent.ForceEndRound += ForceEndTurn;
            ActionPlayerButtonEvent.ActionEndTurnBot += EndTurn;
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
                
                entity.AddComponent(new CardMoveToDiscardComponent());
                entity.AddComponent(new CardPlayAllComponent());
            }
            
            AnimationsMoveAtDiscardDeckAction.AnimationsMoveAtDiscardDeck?.Invoke();
        }

        private void ForceEndTurn()
        {
            var actionButtonUI = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CoreHudUIMono.CoreActionButtonAnimationsMono;
            actionButtonUI.ForceHideButton();
            EndTurn();
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
            ActionPlayerButtonEvent.ClickActionButton -= ClickActionButton;
            ActionPlayerButtonEvent.CheckPlayerHasAnyActionsLeft -= СheckingPlayerHasAnyActionsLeft;
            ActionPlayerButtonEvent.ForceEndRound -= ForceEndTurn;
            ActionPlayerButtonEvent.ActionEndTurnBot -= EndTurn;
        }
    }
}