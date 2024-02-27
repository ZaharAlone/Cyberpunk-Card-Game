using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.InteractiveCard;
using CyberNet.Global;
using DG.Tweening;
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

        public void PreInit()
        {
            RoundAction.EndCurrentTurn += HideButton;
            RoundAction.StartTurn += UpdateButton;
            ActionPlayerButtonEvent.UpdateActionButton += UpdateButton;
            ActionPlayerButtonEvent.ClickActionButton += ClickActionButton;
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
            
            if (roundData.playerOrAI != PlayerOrAI.Player || roundData.PauseInteractive)
            {
                ui.BoardGameUIMono.CoreHudUIMono.HideInteractiveButton();
                return;
            }

            ui.BoardGameUIMono.CoreHudUIMono.ShowInteractiveButton();
            var config = _dataWorld.OneData<BoardGameData>().BoardGameRule;

            var cardHandEntities = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.PlayerID == roundData.CurrentPlayerID)
                .With<CardHandComponent>()
                .GetEntities();
            
            var cardHandCount = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.PlayerID == roundData.CurrentPlayerID)
                .With<CardHandComponent>()
                .Count();

            var isOnlyCardScout = true;

            foreach (var cardHandEntity in cardHandEntities)
            {
                var cardComponent = cardHandEntity.GetComponent<CardComponent>();
                if (cardComponent.Key != "neutral_scout")
                {
                    isOnlyCardScout = false;
                    break;
                }
            }
            
            ref var actionPlayer = ref _dataWorld.OneData<ActionCardData>();
            if (isOnlyCardScout && cardHandCount > 0)
            {
                actionPlayer.ActionPlayerButtonType = ActionPlayerButtonType.PlayAll;
                
                ui.BoardGameUIMono.CoreHudUIMono.SetInteractiveButton(config.ActionPlayAll_loc, config.ActionPlayAll_image);
                ui.BoardGameUIMono.CoreHudUIMono.PopupActionButton.SetKeyPopup(config.PlayAllPopup);
            }
            else
            {
                actionPlayer.ActionPlayerButtonType = ActionPlayerButtonType.EndTurn;
                
                ui.BoardGameUIMono.CoreHudUIMono.SetInteractiveButton(config.ActionEndTurn_loc, config.ActionEndTurn_image);
                ui.BoardGameUIMono.CoreHudUIMono.PopupActionButton.SetKeyPopup(config.EndRoundPopup);
            }
        }
        
        private void HideButton()
        {
            var ui = _dataWorld.OneData<CoreGameUIData>();
            ui.BoardGameUIMono.CoreHudUIMono.HideInteractiveButton();
        }

        private void ClickActionButton()
        {
            ref var actionPlayer = ref _dataWorld.OneData<ActionCardData>();
            
            if (actionPlayer.ActionPlayerButtonType == ActionPlayerButtonType.PlayAll)
                PlayAll();
            else
            {
                EndTurn();
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
                
                entity.AddComponent(new CardMoveToTableComponent());
            }
            
            AnimationsMoveBoardCardAction.AnimationsMoveBoardCard?.Invoke();
            
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
            ActionPlayerButtonEvent.ActionEndTurnBot -= EndTurn;
        }
    }
}