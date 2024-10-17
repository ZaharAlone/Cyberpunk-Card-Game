using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.AbilityCard;
using CyberNet.Global;
using CyberNet.Core.UI.ActionButton;

namespace CyberNet.Core.UI
{
    /// <summary>
    /// Система управляющая Action кнопкой - позволяющей разыгрывать все карты, атаковать, заканчивать раунд
    /// </summary>
    [EcsSystem(typeof(CoreModule))]
    public class ActionPlayerButtonUIViewSystem : IPreInitSystem, IInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        private const string scout_card_name = "neutral_scout";

        public void PreInit()
        {
            RoundAction.EndCurrentTurn += HideButton;
            RoundAction.StartTurn += UpdateButton;
            
            ActionPlayerButtonEvent.UpdateActionButton += UpdateButton;
            ActionPlayerButtonEvent.SetViewBattle += SetViewBattle;
        }

        public void Init()
        {
            ForceHideActionButton();
        }

        private void ForceHideActionButton()
        {
            var actionButton = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.ActionButtonMono.CoreActionButtonAnimationsMono;
            actionButton.ForceHideActionButton();
        }
        
        private void UpdateButton()
        {
            var roundData = _dataWorld.OneData<RoundData>();
            var ui = _dataWorld.OneData<CoreGameUIData>();
            var coreActionButton = ui.BoardGameUIMono.ActionButtonMono.CoreActionButtonAnimationsMono;
            
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
            
            var actionButtonMono = ui.BoardGameUIMono.ActionButtonMono;
            
            if (playAllCard)
            {
                actionPlayer.ActionPlayerButtonType = ActionPlayerButtonType.PlayAll;
                
                actionButtonMono.CoreActionButtonAnimationsMono.SetStateViewButton(ActionPlayerButtonType.PlayAll);
                actionButtonMono.CoreActionButtonAnimationsMono.SetAnimationsReadyClick();
                actionButtonMono.PopupActionButton.SetKeyPopup(boardGameRule.PlayAllPopup);
            }
            else
            {
                actionPlayer.ActionPlayerButtonType = ActionPlayerButtonType.EndTurn;
                actionButtonMono.CoreActionButtonAnimationsMono.SetStateViewButton(ActionPlayerButtonType.EndTurn);
                actionButtonMono.PopupActionButton.SetKeyPopup(boardGameRule.EndRoundPopup);

                if (endRoundNoActive)
                {
                    actionButtonMono.CoreActionButtonAnimationsMono.SetAnimationsNotReadyButtonClick();
                }
                else
                {
                    actionButtonMono.CoreActionButtonAnimationsMono.SetAnimationsReadyClick();
                }
            }
        }

        private void SetViewBattle()
        {
            ref var actionPlayer = ref _dataWorld.OneData<ActionCardData>();
            var ui = _dataWorld.OneData<CoreGameUIData>();
            var boardGameRule = _dataWorld.OneData<BoardGameData>().BoardGameRule;
            var actionButtonMono = ui.BoardGameUIMono.ActionButtonMono;

            actionPlayer.ActionPlayerButtonType = ActionPlayerButtonType.Attack;
            
            actionButtonMono.CoreActionButtonAnimationsMono.SetStateViewButton(ActionPlayerButtonType.Attack);
            actionButtonMono.CoreActionButtonAnimationsMono.SetAnimationsReadyClick();
            actionButtonMono.PopupActionButton.SetKeyPopup(boardGameRule.AttackArenaPopup);
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
            ui.BoardGameUIMono.ActionButtonMono.CoreActionButtonAnimationsMono.HideButtonPlayAnimations();
        }

        public void Destroy()
        {
            RoundAction.EndCurrentTurn -= HideButton;
            RoundAction.StartTurn -= UpdateButton;
            
            ActionPlayerButtonEvent.UpdateActionButton -= UpdateButton;
            ActionPlayerButtonEvent.SetViewBattle -= SetViewBattle;
        }
    }
}