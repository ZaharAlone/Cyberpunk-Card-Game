using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.AbilityCard.UI;
using CyberNet.Core.InteractiveCard;
using CyberNet.Core.UI;
using Input;
using UnityEngine;
using DG.Tweening;

namespace CyberNet.Core
{
    /// <summary>
    /// Система отвечает за UI выбора абилки карты, если абилка одна, ничего не происходит. Две - открывается UI выбора
    /// </summary>
    [EcsSystem(typeof(CoreModule))]
    public class SelectAbilitySystem : IPreInitSystem, IRunSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            //Follow button select ability
            SelectAbilityAction.SelectFirstAbility += OnClickSelectFirstAbility;
            SelectAbilityAction.SelectSecondAbility += OnClickSelectSecondAbility;
        }

        public void Run()
        {
            //Add cancel button select ability
            var selectPlayerAbilityCard = _dataWorld.Select<SelectingPlayerAbilityComponent>().Count();
            if (selectPlayerAbilityCard != 0)
            {
                var inputData = _dataWorld.OneData<InputData>();
                if (inputData.RightClick || inputData.ExitUI)
                    CancelSelectAbility();
                return;   
            }
            
            var cardSelectPlayerAbilityCard = _dataWorld.Select<NeedToSelectAbilityCardComponent>().Count();   
            if (cardSelectPlayerAbilityCard == 0)
                return;
            
            var entities = _dataWorld.Select<NeedToSelectAbilityCardComponent>().GetEntities();

            foreach (var entity in entities)
            {
                var isOneAbility = SelectAbility(entity);
                if (!isOneAbility)
                {
                    break;
                }
            }
        }

        private void CancelSelectAbility()
        {
            var entityCard = _dataWorld.Select<CardComponent>()
                .With<SelectingPlayerAbilityComponent>()
                .SelectFirstEntity();

            entityCard.RemoveComponent<SelectingPlayerAbilityComponent>();
            entityCard.RemoveComponent<NeedToSelectAbilityCardComponent>();
            
            _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.SelectAbilityUIMono.CloseFrame();
            AbilityInputButtonUIAction.HideInputUIButton?.Invoke();
            CardAnimationsHandAction.AnimationsFanCardInHand?.Invoke();
        }
        
        private bool SelectAbility(Entity entity)
        {
            var cardComponent = entity.GetComponent<CardComponent>();
            var isOneAbility = AbilityCardUtilsAction.CalculateHowManyAbilitiesAvailableForSelection.Invoke(cardComponent) < 2;
            
            if (isOneAbility)
            {
                entity.RemoveComponent<NeedToSelectAbilityCardComponent>();
                entity.AddComponent(new CardAbilitySelectionCompletedComponent
                { 
                    SelectAbility = SelectAbilityEnum.Ability_0,
                    OneAbilityInCard = true
                });
                
                InteractiveActionCard.FinishSelectAbilityCard?.Invoke(cardComponent.GUID);
            }
            else
            {
                entity.AddComponent(new SelectingPlayerAbilityComponent());
                OpenUISelectAbilityCard(cardComponent);
                AbilityInputButtonUIAction.ShowCancelButton?.Invoke(false);
                AnimationShowCard(entity);
                return isOneAbility;
            }
            
            return isOneAbility;
        }
        
        private void AnimationShowCard(Entity entity)
        {
            entity.RemoveComponent<InteractiveSelectCardComponent>();
            
            if (!entity.HasComponent<CardComponentAnimations>())
                return;
                
            ref var animationsCard = ref entity.GetComponent<CardComponentAnimations>();
            ref var cardComponent = ref entity.GetComponent<CardComponent>();

            var targetPosition = animationsCard.Positions;
            targetPosition.y += 75;
            animationsCard.Sequence.Kill();
            animationsCard.Sequence = DOTween.Sequence();
            animationsCard.Sequence.Append(cardComponent.RectTransform.DOLocalRotateQuaternion(animationsCard.Rotate, 0.3f))
                .Join(cardComponent.RectTransform.DOAnchorPos(targetPosition, 0.3f))
                .Join(cardComponent.RectTransform.DOScale(new Vector3(1f, 1f,1f), 0.3f));
        }

        private void OpenUISelectAbilityCard(CardComponent cardComponent)
        {
            var uiSelectAbility = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.SelectAbilityUIMono;
            var cardsConfig = _dataWorld.OneData<CardsConfig>();

            var boardGameData = _dataWorld.OneData<BoardGameData>();
            var cardsViewConfig = boardGameData.CardsViewConfig;

            cardsConfig.Cards.TryGetValue(cardComponent.Key, out var cardConfig);
            
            cardsViewConfig.CardsImageDictionary.TryGetValue(cardConfig.ImageKey, out var cardImage);
            
            uiSelectAbility.LeftCard.SetViewCard(cardImage, cardConfig.Header, cardConfig.Price, cardConfig.ValueLeftPoint, cardConfig.ValueRightPoint);
            uiSelectAbility.RightCard.SetViewCard(cardImage, cardConfig.Header, cardConfig.Price, cardConfig.ValueLeftPoint, cardConfig.ValueRightPoint);
            //TODO разобраться
            SetViewAbilityCard.SetView(uiSelectAbility.LeftCard.AbilityContainer, cardConfig.Ability_0, boardGameData, cardsConfig, false, false);
            SetViewAbilityCard.SetView(uiSelectAbility.RightCard.AbilityContainer, cardConfig.Ability_1, boardGameData, cardsConfig, false, false);
            uiSelectAbility.OpenFrame();
        }
        
        private void OnClickSelectFirstAbility()
        {
            SelectConfimAbility(SelectAbilityEnum.Ability_0);
        }

        private void OnClickSelectSecondAbility()
        {
            SelectConfimAbility(SelectAbilityEnum.Ability_1);
        }

        private void SelectConfimAbility(SelectAbilityEnum targetAbility)
        {
            var entity = _dataWorld.Select<NeedToSelectAbilityCardComponent>()
                .With<SelectingPlayerAbilityComponent>()
                .SelectFirstEntity();
            
            entity.RemoveComponent<NeedToSelectAbilityCardComponent>();
            entity.RemoveComponent<SelectingPlayerAbilityComponent>();
            
            entity.AddComponent(new CardAbilitySelectionCompletedComponent { SelectAbility = targetAbility});
            
            var uiSelectAbility = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.SelectAbilityUIMono;
            uiSelectAbility.CloseFrame();

            var cardComponent = entity.GetComponent<CardComponent>();
            InteractiveActionCard.FinishSelectAbilityCard?.Invoke(cardComponent.GUID);
        }

        public void Destroy()
        {
            SelectAbilityAction.SelectFirstAbility -= OnClickSelectFirstAbility;
            SelectAbilityAction.SelectSecondAbility -= OnClickSelectSecondAbility;
        }
    }
}