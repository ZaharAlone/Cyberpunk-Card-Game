using CyberNet.Core.AbilityCard;
using CyberNet.Core.AbilityCard.UI;
using CyberNet.Core.Arena.ArenaHUDUI;
using CyberNet.Core.BezierCurveNavigation;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.City;
using CyberNet.Core.InteractiveCard;
using CyberNet.Core.UI;
using CyberNet.Core.UI.CorePopup;
using Input;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CyberNet.Core.Arena
{
    [EcsSystem(typeof(ArenaModule))]
    public class ArenaUnitAttackSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            ArenaAction.ArenaUnitStartAttack += ArenaUnitStartAttack;
        }
        
        private void ArenaUnitStartAttack()
        {
            var currentUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaUnitCurrentComponent>()
                .SelectFirstEntity();
            var currentUnitComponent = currentUnitEntity.GetComponent<ArenaUnitComponent>();
            
            var targetUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaSelectUnitForAttackComponent>()
                .SelectFirstEntity();
            var targetUnitComponent = targetUnitEntity.GetComponent<ArenaUnitComponent>();

            currentUnitComponent.UnitArenaMono.ShowTargetUnit(targetUnitComponent.UnitGO.transform);

            Attack();
        }

        private void Attack()
        {
            if (CheckBlockAttack())
            {
                AbilityInputButtonUIAction.ShowTakeDamageBattleButton?.Invoke();
                VFXCardInteractiveAction.EnableVFXAllCardInHand?.Invoke();
                _dataWorld.OneData<RoundData>().PauseInteractive = false;
                
                InteractiveActionCard.StartInteractiveCard += DownClickCard;
                InputAction.RightMouseButtonClick += CancelSelectCard;
            }
            else
            {
                AttackUnitPlayer();
            }
        }

        private void DownClickCard(string guidCard)
        {
            _dataWorld.OneData<RoundData>().PauseInteractive = true;
            var entityCard = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == guidCard)
                .SelectFirstEntity();
            
            entityCard.RemoveComponent<CardHandComponent>();
            entityCard.RemoveComponent<InteractiveSelectCardComponent>();
            entityCard.RemoveComponent<CardComponentAnimations>();
            entityCard.AddComponent(new CardMoveToTableComponent());
            
            CardAnimationsHandAction.AnimationsFanCardInHand?.Invoke();
            AnimationsMoveBoardCardAction.AnimationsMoveBoardCard?.Invoke();   
            
            CityAction.UpdateCanInteractiveMap?.Invoke();
            AbilityInputButtonUIAction.HideInputUIButton?.Invoke();
            
            InteractiveActionCard.StartInteractiveCard -= DownClickCard;
            InputAction.RightMouseButtonClick -= CancelSelectCard;
            CoreElementInfoPopupAction.ClosePopupCard?.Invoke();
            
            ArenaAction.ArenaUnitFinishAttack += FinishBlockAttack;
            
            var targetUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaSelectUnitForAttackComponent>()
                .SelectFirstEntity();
            var targetUnitComponent = targetUnitEntity.GetComponent<ArenaUnitComponent>();
            targetUnitComponent.UnitArenaMono.OnShield();
            
            var currentUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaUnitCurrentComponent>()
                .SelectFirstEntity();
            var currentUnitComponent = currentUnitEntity.GetComponent<ArenaUnitComponent>();
            currentUnitComponent.UnitArenaMono.Shooting();
        }

        private void FinishBlockAttack()
        {
            ArenaAction.ArenaUnitFinishAttack -= FinishBlockAttack;
            
            var targetUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaSelectUnitForAttackComponent>()
                .SelectFirstEntity();
            var targetUnitComponent = targetUnitEntity.GetComponent<ArenaUnitComponent>();
            targetUnitComponent.UnitArenaMono.OffShield();
            
            ArenaAction.FinishRound?.Invoke();
            ArenaUIAction.StartNewRoundUpdateOrderPlayer?.Invoke();
        }

        private void CancelSelectCard()
        {
            InteractiveActionCard.StartInteractiveCard -= DownClickCard;
            InputAction.RightMouseButtonClick -= CancelSelectCard;
            
            AbilityInputButtonUIAction.HideInputUIButton?.Invoke();
            VFXCardInteractiveAction.UpdateVFXCard?.Invoke();
            _dataWorld.OneData<RoundData>().PauseInteractive = true;
            
            AttackUnitPlayer();
        }
        
        private void AttackUnitPlayer()
        {
            var currentUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaUnitCurrentComponent>()
                .SelectFirstEntity();
            var currentUnitComponent = currentUnitEntity.GetComponent<ArenaUnitComponent>();
            currentUnitComponent.UnitArenaMono.Shooting();
            ArenaAction.ArenaUnitFinishAttack += ArenaUnitFinishAttack;
        }

        private bool CheckBlockAttack()
        {
            var targetUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaSelectUnitForAttackComponent>()
                .SelectFirstEntity();
            var targetUnitComponent = targetUnitEntity.GetComponent<ArenaUnitComponent>();

            var countCardInHandPlayer = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.PlayerID == targetUnitComponent.PlayerControlID)
                .With<CardHandComponent>()
                .Count();

            return countCardInHandPlayer > 0;
        } 
        
        private void ArenaUnitFinishAttack()
        {
            var targetUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaSelectUnitForAttackComponent>()
                .SelectFirstEntity();
            var targetUnitComponent = targetUnitEntity.GetComponent<ArenaUnitComponent>();
            var targetUnitMapComponent = targetUnitEntity.GetComponent<UnitMapComponent>();
            
            Object.Destroy(targetUnitComponent.UnitGO);
            Object.Destroy(targetUnitMapComponent.UnitIconsGO);
            
            targetUnitEntity.Destroy();
            
            ArenaAction.FinishRound?.Invoke();
            ArenaUIAction.StartNewRoundUpdateOrderPlayer?.Invoke();
            ArenaAction.ArenaUnitFinishAttack -= ArenaUnitFinishAttack;
        }

        public void Destroy()
        {
            ArenaAction.ArenaUnitStartAttack -= ArenaUnitStartAttack;
            ArenaAction.ArenaUnitFinishAttack -= ArenaUnitFinishAttack;
        }
    }
}