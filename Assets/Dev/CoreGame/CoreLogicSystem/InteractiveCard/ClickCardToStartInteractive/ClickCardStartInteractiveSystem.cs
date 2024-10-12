using CyberNet.Core.AbilityCard;
using CyberNet.Core.AbilityCard.DiscardCard;
using CyberNet.Core.Battle.TacticsMode;
using CyberNet.Core.Battle.TacticsMode.InteractiveCard;
using CyberNet.Core.Player;
using CyberNet.Core.UI;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.UI.CorePopup;
using CyberNet.Global.Sound;
using DG.Tweening;
using Input;
using UnityEngine;

namespace CyberNet.Core.InteractiveCard
{
    [EcsSystem(typeof(CoreModule))]
    public class ClickCardStartInteractiveSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            InteractiveActionCard.StartInteractiveCard += DownClickCard;
            BattleTacticsUIAction.StartMoveCardTactics += StartInteractiveCardTactics;
            InteractiveActionCard.FinishSelectAbilityCard += FinishSelectAbilityCard;
        }

        private void DownClickCard(string guid)
        {
            if (CheckPlayerDiscardCard())
                return;

            ref var roundData = ref _dataWorld.OneData<RoundData>();
            if (roundData.PauseInteractive)
                return;
            
            var entity = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == guid)
                .SelectFirstEntity();
            
            if (entity.HasComponent<CardHandComponent>())
            {
                var cardComponent = entity.GetComponent<CardComponent>();
                var isAbilityPlaying = AbilityCardUtilsAction.CalculateHowManyAbilitiesAvailableForSelection.Invoke(cardComponent) > 0;
                
                if (!isAbilityPlaying)
                    return;
                
                CoreElementInfoPopupAction.ClosePopupCard?.Invoke();
                entity.AddComponent(new NeedToSelectAbilityCardComponent());
            }
        }

        private bool CheckPlayerDiscardCard()
        {
            var playerIsDiscardCard = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .With<PlayerIsDiscardsCardComponent>()
                .Count() > 0;
            return playerIsDiscardCard;
        }

        private void FinishSelectAbilityCard(string guid)
        {
            var entity = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == guid)
                .SelectFirstEntity();

            var cardComponent = entity.GetComponent<CardComponent>();
            var selectAbility = entity.GetComponent<CardAbilitySelectionCompletedComponent>().SelectAbility;
            
            var configAbility = _dataWorld.OneData<CardsConfig>().AbilityCard;
            var cardAbility = new AbilityCardContainer();
           
            if (selectAbility == SelectAbilityEnum.Ability_0)
                cardAbility = cardComponent.Ability_0;
            else
                cardAbility = cardComponent.Ability_1;
            
            configAbility.TryGetValue(cardAbility.AbilityType.ToString(), out var abilityCardConfig);
            
            ApplyAbilityCard(guid, abilityCardConfig.VisualPlayingCard);
        }
        
        private void ApplyAbilityCard(string guid, VisualPlayingCardType visualPlayingCardType)
        {
            var cardEntity = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == guid)
                .SelectFirstEntity();

            var selectAbility = cardEntity.GetComponent<CardAbilitySelectionCompletedComponent>();
            
            if (visualPlayingCardType == VisualPlayingCardType.Table)
            {
                if (selectAbility.OneAbilityInCard)
                {
                    AddMoveCardComponent(cardEntity);
                }
                else
                {
                    cardEntity.AddComponent(new CardMoveToDiscardComponent());
                    cardEntity.RemoveComponent<CardComponentAnimations>();
                    cardEntity.RemoveComponent<CardHandComponent>();
                    
                    AnimationsMoveAtDiscardDeckAction.AnimationsMoveAtDiscardDeck?.Invoke();
                    CardAnimationsHandAction.AnimationsFanCardInHand?.Invoke();
                }
            }
            else if (visualPlayingCardType == VisualPlayingCardType.Target)
            {
                cardEntity.AddComponent(new SelectTargetCardAbilityComponent());
                SelectTargetCardAbilityUIAction.SelectTarget?.Invoke();
                AbilityCardAction.UpdateValueResourcePlayedCard?.Invoke();
            }
        }

        private void StartInteractiveCardTactics(string guidCard)
        {
            var cardEntity = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == guidCard)
                .With<CardTacticsComponent>()
                .SelectFirstEntity();

            AddMoveCardComponent(cardEntity);
            BattleTacticsUIAction.CheckIsSelectCardTactics?.Invoke(guidCard);
        }
        
        private void AddMoveCardComponent(Entity entity)
        {
            var mousePositions = InputAction.GetCurrentMousePositionsToScreen.Invoke();
            ref var component = ref entity.GetComponent<CardComponent>();

            if (entity.HasComponent<CardComponentAnimations>())
            {
                var cardAnimationsComponent = entity.GetComponent<CardComponentAnimations>();
                cardAnimationsComponent.Sequence.Kill();
            }
            
            component.RectTransform.localRotation = Quaternion.identity;
            
            entity.AddComponent(new InteractiveMoveComponent
            {
                StartCardPosition = component.RectTransform.anchoredPosition,
                StartCardRotation = component.RectTransform.localRotation,
                StartMousePositions = mousePositions,
            });
            entity.AddComponent(new CardMoveInStartZoneComponent());
            CoreElementInfoPopupAction.ClosePopupCard?.Invoke();

            var startMoveSFX = _dataWorld.OneData<SoundData>().Sound.StartInteractiveCard;
            SoundAction.PlaySound?.Invoke(startMoveSFX);

            _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.BlockRaycastPanel.SetActive(true);
        }
        
        public void Destroy()
        {
            InteractiveActionCard.StartInteractiveCard -= DownClickCard;
            InteractiveActionCard.FinishSelectAbilityCard -= FinishSelectAbilityCard;
            BattleTacticsUIAction.StartMoveCardTactics -= StartInteractiveCardTactics;
        }
    }
}