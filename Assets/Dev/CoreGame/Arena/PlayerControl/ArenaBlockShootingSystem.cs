using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.AbilityCard.UI;
using CyberNet.Core.Arena;
using CyberNet.Core.InteractiveCard;
using CyberNet.Core.Player;
using CyberNet.Core.UI;
using CyberNet.Core.UI.CorePopup;
using CyberNet.Global;
using Input;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class ArenaBlockShootingSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            ArenaAction.StartInteractiveBlockingShooting += StartBlockShooting;
        }

        private void StartBlockShooting()
        {
            var targetUnitComponent = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaSelectUnitForAttackComponent>()
                .SelectFirst<ArenaUnitComponent>();
            
            var playerComponent = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player => player.PlayerID == targetUnitComponent.PlayerControlID)
                .SelectFirst<PlayerComponent>();

            if (playerComponent.playerOrAI == PlayerOrAI.Player)
            {
                PlayerBlockingShootingInitLogic();
            }
            else
            {
                AIBlockingShootingLogic();
            }
        }

        private void PlayerBlockingShootingInitLogic()
        {
            AbilityInputButtonUIAction.ShowTakeDamageBattleButton?.Invoke();
            VFXCardInteractiveAction.EnableVFXAllCardInHand?.Invoke();
            _dataWorld.OneData<RoundData>().PauseInteractive = false;
                
            InteractiveActionCard.StartInteractiveCard += DownClickCard;
            InputAction.RightMouseButtonClick += CancelSelectCard;
        }
        
        private void DownClickCard(string guidCard)
        {
            _dataWorld.OneData<RoundData>().PauseInteractive = true;
            var entityCard = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == guidCard)
                .SelectFirstEntity();

            var cardComponent = entityCard.GetComponent<CardComponent>();
            cardComponent.CardMono.SetStatusInteractiveVFX(false);
            
            entityCard.RemoveComponent<CardHandComponent>();
            entityCard.RemoveComponent<InteractiveSelectCardComponent>();
            entityCard.RemoveComponent<CardComponentAnimations>();
            entityCard.AddComponent(new CardStartMoveToTableComponent());
            
            CardAnimationsHandAction.AnimationsFanCardInHand?.Invoke();
            AnimationsMoveBoardCardAction.AnimationsMoveBoardCard?.Invoke();   
            
            AbilityInputButtonUIAction.HideInputUIButton?.Invoke();
            
            InteractiveActionCard.StartInteractiveCard -= DownClickCard;
            InputAction.RightMouseButtonClick -= CancelSelectCard;
            CoreElementInfoPopupAction.ClosePopupCard?.Invoke();

            UnitOnShield();

            _dataWorld.NewEntity().AddComponent(new UnitOnShieldComponent());

            ArenaAction.StartShootingPlayerWithShield?.Invoke();
        }
        
        private void UnitOnShield()
        {
            var targetUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaSelectUnitForAttackComponent>()
                .SelectFirstEntity();
            var targetUnitComponent = targetUnitEntity.GetComponent<ArenaUnitComponent>();
            targetUnitComponent.UnitArenaMono.OnShield();
        }
        
        private void CancelSelectCard()
        {
            InteractiveActionCard.StartInteractiveCard -= DownClickCard;
            InputAction.RightMouseButtonClick -= CancelSelectCard;
            
            AbilityInputButtonUIAction.HideInputUIButton?.Invoke();
            VFXCardInteractiveAction.UpdateVFXCard?.Invoke();
            _dataWorld.OneData<RoundData>().PauseInteractive = true;
            
            ArenaAction.StartShootingPlayerWithoutShield?.Invoke();
        }

        private void AIBlockingShootingLogic()
        {
            var aiIsBlockingShooting = ArenaAction.CheckReactionsShooting.Invoke();

            if (aiIsBlockingShooting)
            {
                UnitOnShield();
                ArenaAction.StartShootingPlayerWithShield?.Invoke();
            }
            else
            {
                ArenaAction.StartShootingPlayerWithoutShield?.Invoke();
            }
        }

        public void Destroy()
        {
            ArenaAction.StartInteractiveBlockingShooting -= StartBlockShooting;
        }
    }
}