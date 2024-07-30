using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.AbilityCard.UI;
using CyberNet.Core.AI;
using CyberNet.Core.Arena;
using CyberNet.Core.BezierCurveNavigation;
using CyberNet.Core.City;
using CyberNet.Core.InteractiveCard;
using CyberNet.Core.Player;
using CyberNet.Core.UI;
using CyberNet.Global;
using Input;

namespace CyberNet.Core.AbilityCard
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityHeadshotSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AbilityCardAction.HeadShot += HeadShot;
        }
        
        private void HeadShot(string guidCard)
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();

            if (roundData.playerOrAI != PlayerOrAI.Player)
            {
                //AbilityAIAction.HeadShot?.Invoke(guidCard);
                return;
            }
            
            Debug.LogError("Ability head shot");
            AbilitySelectElementUIAction.OpenSelectAbilityCard?.Invoke(AbilityType.HeadShot, 0, false);
            BezierCurveNavigationAction.StartBezierCurveCard?.Invoke(guidCard, BezierTargetEnum.ArenaUnit);

            InputAction.LeftMouseButtonClick += ClickMouse;
        }
        
        private void ClickMouse()
        {
            var bezierComponent = _dataWorld.Select<BezierCurveNavigationComponent>()
                .SelectFirstEntity()
                .GetComponent<BezierCurveNavigationComponent>();

            if (bezierComponent.SelectTarget && bezierComponent.Target == BezierTargetEnum.ArenaUnit)
            {
                var selectTargetUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                    .Where<ArenaUnitComponent>(unit => unit.GUID == bezierComponent.GUIDTarget)
                    .SelectFirstEntity();

                var currentUnitIsTargetAim = selectTargetUnitEntity.HasComponent<ArenaSelectUnitForAttackComponent>();
                
                ArenaAction.KillUnitGUID?.Invoke(bezierComponent.GUIDTarget);

                if (currentUnitIsTargetAim)
                {
                    ArenaAction.SelectUnitEnemyTargetingPlayer?.Invoke();
                }

                EndPlayingCard();
            }
        }

        private void EndPlayingCard()
        {
            var entityCard = _dataWorld.Select<CardComponent>()
                .With<AbilityCardMoveUnitComponent>()
                .SelectFirstEntity();

            entityCard.RemoveComponent<SelectTargetCardAbilityComponent>();
            entityCard.RemoveComponent<CardHandComponent>();
            entityCard.RemoveComponent<InteractiveSelectCardComponent>();
            entityCard.RemoveComponent<CardComponentAnimations>();
            entityCard.RemoveComponent<AbilitySelectElementComponent>();
            
            entityCard.AddComponent(new CardStartMoveToTableComponent());

            CardAnimationsHandAction.AnimationsFanCardInHand?.Invoke();
            AnimationsMoveBoardCardAction.AnimationsMoveBoardCard?.Invoke();   
            
            AbilitySelectElementUIAction.ClosePopup?.Invoke();
            AbilityInputButtonUIAction.HideInputUIButton?.Invoke();
        }

        public void Destroy()
        {
            AbilityCardAction.HeadShot -= HeadShot;
        }
    }
}