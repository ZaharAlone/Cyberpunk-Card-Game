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
            var isOneAbility = false;
            
            if (cardComponent.Ability_1.AbilityType == AbilityType.None)
            {
                entity.RemoveComponent<NeedToSelectAbilityCardComponent>();
                entity.AddComponent(new CardAbilitySelectionCompletedComponent
                { 
                    SelectAbility = SelectAbilityEnum.Ability_0,
                    OneAbilityInCard = true
                });
                isOneAbility = true;
                
                InteractiveActionCard.FinishSelectAbilitycard?.Invoke(cardComponent.GUID);
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
            var boardGameConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig;
            var cardsImage = _dataWorld.OneData<BoardGameData>().CardsImage;
            var cardsConfig = _dataWorld.OneData<CardsConfig>();
            cardsConfig.Cards.TryGetValue(cardComponent.Key, out var cardConfig);
            
            cardsImage.TryGetValue(cardConfig.ImageKey, out var cardImage);
            
            boardGameConfig.NationsImage.TryGetValue(cardConfig.Nations, out var nationsImage);
            uiSelectAbility.LeftCard.SetViewCard(cardImage, cardConfig.Header, cardConfig.Price, cardConfig.CyberpsychosisCount, nationsImage);
            uiSelectAbility.RightCard.SetViewCard(cardImage, cardConfig.Header, cardConfig.Price, cardConfig.CyberpsychosisCount, nationsImage);
            
            SetViewAbilityCard.SetView(uiSelectAbility.LeftCard.AbilityContainer, cardConfig.Ability_0, boardGameConfig, cardsConfig, false, true);
            SetViewAbilityCard.SetView(uiSelectAbility.RightCard.AbilityContainer, cardConfig.Ability_1, boardGameConfig, cardsConfig, false, true);
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
            var entity = _dataWorld.Select<NeedToSelectAbilityCardComponent>().With<SelectingPlayerAbilityComponent>().SelectFirstEntity();
            entity.RemoveComponent<NeedToSelectAbilityCardComponent>();
            entity.RemoveComponent<SelectingPlayerAbilityComponent>();
            entity.AddComponent(new CardAbilitySelectionCompletedComponent { SelectAbility = targetAbility});
            
            var uiSelectAbility = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.SelectAbilityUIMono;
            uiSelectAbility.CloseFrame();

            var cardComponent = entity.GetComponent<CardComponent>();
            InteractiveActionCard.FinishSelectAbilitycard?.Invoke(cardComponent.GUID);
            AbilityInputButtonUIAction.HideInputUIButton?.Invoke();
        }

        public void Destroy()
        {
            SelectAbilityAction.SelectFirstAbility -= OnClickSelectFirstAbility;
            SelectAbilityAction.SelectSecondAbility -= OnClickSelectSecondAbility;
        }
    }
}