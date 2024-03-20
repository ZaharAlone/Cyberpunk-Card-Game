using CyberNet.Core.AbilityCard.UI;
using CyberNet.Core.Arena.ArenaHUDUI;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.City;
using CyberNet.Core.InteractiveCard;
using CyberNet.Core.UI;
using CyberNet.Core.UI.CorePopup;
using CyberNet.Global;
using CyberNet.Global.Sound;
using Input;
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

            currentUnitComponent.UnitArenaMono.OnAimAnimations();
            currentUnitComponent.UnitArenaMono.ViewToTargetUnit(targetUnitComponent.UnitGO.transform);

            var soundAim = _dataWorld.OneData<SoundData>().Sound.AimGun;
            SoundAction.PlaySound?.Invoke(soundAim);
            
            _dataWorld.NewEntity().AddComponent(new TimeComponent
            {
                Time = 0.5f,
                Action = () => Attack()
            });
        }

        private void Attack()
        {
            if (ArenaAction.CheckBlockAttack.Invoke())
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

            var cardComponent = entityCard.GetComponent<CardComponent>();
            cardComponent.CardMono.SetStatusInteractiveVFX(false);
            
            entityCard.RemoveComponent<CardHandComponent>();
            entityCard.RemoveComponent<InteractiveSelectCardComponent>();
            entityCard.RemoveComponent<CardComponentAnimations>();
            entityCard.AddComponent(new CardMoveToTableComponent());
            
            CardAnimationsHandAction.AnimationsFanCardInHand?.Invoke();
            AnimationsMoveBoardCardAction.AnimationsMoveBoardCard?.Invoke();   
            
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
            
            Shooting();
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
            Shooting();
            ArenaAction.ArenaUnitFinishAttack += ArenaUnitFinishAttack;
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

        public void Shooting()
        {
            var currentUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaUnitCurrentComponent>()
                .SelectFirstEntity();
            var currentUnitComponent = currentUnitEntity.GetComponent<ArenaUnitComponent>();
            currentUnitComponent.UnitArenaMono.StartShooting();
            
            UnitArenaAction.GunShootingVFX += ShootingGunPlayVFX;
            _dataWorld.NewEntity().AddComponent(new TimeComponent {
                Time = 1.3f,
                Action = () => FinishShooting()
            });
        }

        public void ShootingGunPlayVFX()
        {
            var currentUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaUnitCurrentComponent>()
                .SelectFirstEntity();
            var currentUnitComponent = currentUnitEntity.GetComponent<ArenaUnitComponent>();
            currentUnitComponent.UnitArenaMono.ShootingGunPlayVFX();

            UnitArenaAction.CreateBulletCurrentUnit?.Invoke();
            var soundShoot = _dataWorld.OneData<SoundData>().Sound.Shoot;
            SoundAction.PlaySound?.Invoke(soundShoot);
        }

        public void FinishShooting()
        {
            UnitArenaAction.GunShootingVFX -= ShootingGunPlayVFX;
            var currentUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaUnitCurrentComponent>()
                .SelectFirstEntity();
            var currentUnitComponent = currentUnitEntity.GetComponent<ArenaUnitComponent>();
            currentUnitComponent.UnitArenaMono.FinishShooting();
        }

        public void Destroy()
        {
            ArenaAction.ArenaUnitStartAttack -= ArenaUnitStartAttack;
            ArenaAction.ArenaUnitFinishAttack -= ArenaUnitFinishAttack;
        }
    }
}