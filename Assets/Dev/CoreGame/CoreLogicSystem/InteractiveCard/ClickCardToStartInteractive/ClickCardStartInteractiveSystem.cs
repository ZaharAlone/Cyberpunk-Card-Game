using CyberNet.Core.AbilityCard;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.UI.CorePopup;
using Input;

namespace CyberNet.Core.InteractiveCard
{
    [EcsSystem(typeof(CoreModule))]
    public class ClickCardStartInteractiveSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            InteractiveActionCard.StartInteractiveCard += DownClickCard;
            InteractiveActionCard.FinishSelectAbilitycard += FinishSelectAbilityCard;
        }

        private void DownClickCard(string guid)
        {
            var entity = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == guid)
                .SelectFirstEntity();
            
            ref var roundData = ref _dataWorld.OneData<RoundData>();
            if (roundData.PauseInteractive)
                return;
            
            if (entity.HasComponent<CardTradeRowComponent>() && entity.HasComponent<CardFreeToBuyComponent>())
            {
                AddMoveCardComponent(entity);
                return;
            }
            
            if (entity.HasComponent<CardHandComponent>())
            {
                CoreElementInfoPopupAction.ClosePopupCard?.Invoke();
                entity.AddComponent(new NeedToSelectAbilityCardComponent());
            }
        }

        private void FinishSelectAbilityCard(string guid)
        {
            var entity = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == guid)
                .SelectFirstEntity();

            var cardComponent = entity.GetComponent<CardComponent>();
            var selectAbility = entity.GetComponent<CardAbilitySelectionCompletedComponent>().SelectAbility;
            var currentRoundState = _dataWorld.OneData<RoundData>().CurrentRoundState;
            
            var configAbility = _dataWorld.OneData<CardsConfig>().AbilityCard;
            var cardAbility = new AbilityCardContainer();
           
            if (selectAbility == SelectAbilityEnum.Ability_0)
            {
                cardAbility = cardComponent.Ability_0;
            }
            else
            {
                cardAbility = cardComponent.Ability_1;
            }
            configAbility.TryGetValue(cardAbility.AbilityType.ToString(), out var abilityCardConfig);
            
            if (currentRoundState == RoundState.Map)
            {
                ApplyAbilityCard(guid, cardAbility, abilityCardConfig.VisualPlayingCardMap);
            }
            else
            {
                ApplyAbilityCard(guid, cardAbility, abilityCardConfig.VisualPlayingCardArena);
            }
        }
        
        private void ApplyAbilityCard(string guid, AbilityCardContainer abilityCard, VisualPlayingCardType visualPlayingCardType)
        {
            var entity = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == guid)
                .SelectFirstEntity();

            var selectAbility = entity.GetComponent<CardAbilitySelectionCompletedComponent>();
            
            if (visualPlayingCardType == VisualPlayingCardType.Table)
            {
                if (selectAbility.OneAbilityInCard)
                {
                    AddMoveCardComponent(entity);
                }
                else
                {
                    entity.AddComponent(new CardMoveToTableComponent());
                    AnimationsMoveBoardCardAction.AnimationsMoveBoardCard?.Invoke();   
                }
            }
            else if (visualPlayingCardType == VisualPlayingCardType.Target)
            {
                entity.AddComponent(new SelectTargetCardAbilityComponent());
                SelectTargetCardAbilityAction.SelectTarget?.Invoke();
                AbilityCardAction.UpdateValueResourcePlayedCard?.Invoke();
            }
        }
        
        private void AddMoveCardComponent(Entity entity)
        {
            ref var inputData = ref _dataWorld.OneData<InputData>();
            ref var component = ref entity.GetComponent<CardComponent>();
            
            entity.AddComponent(new InteractiveMoveComponent
            {
                StartCardPosition = component.RectTransform.anchoredPosition,
                StartCardRotation = component.RectTransform.localRotation,
                StartMousePositions = inputData.MousePosition
            });
            CoreElementInfoPopupAction.ClosePopupCard?.Invoke();
        }
        
        public void Destroy()
        {
            InteractiveActionCard.StartInteractiveCard -= DownClickCard;
        }
    }
}