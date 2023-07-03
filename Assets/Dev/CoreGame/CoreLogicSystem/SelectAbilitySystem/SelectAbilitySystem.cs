using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using System.Threading.Tasks;
using CyberNet.Core.UI;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class SelectAbilitySystem : IPreInitSystem, IRunSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            SelectAbilityAction.SelectFirstAbility += SelectFirstAbility;
            SelectAbilityAction.SelectSecondAbility += SelectSecondAbility;
        }

        public void Run()
        {
            var selectPlayerAbilityCard = _dataWorld.Select<SelectPlayerAbilityComponent>().Count();
            if (selectPlayerAbilityCard != 0)
                return;
            
            var entities = _dataWorld.Select<CardSelectAbilityComponent>().GetEntities();

            foreach (var entity in entities)
            {
                var isOneAbility = SelectAbility(entity);
                if (!isOneAbility)
                    break;
            }
        }

        private bool SelectAbility(Entity entity)
        {
            var cardComponent = entity.GetComponent<CardComponent>();
            var isOneAbility = false;
            
            if (cardComponent.Ability_1.AbilityType == AbilityType.None)
            {
                entity.RemoveComponent<CardSelectAbilityComponent>();
                entity.AddComponent(new CardTableComponent { SelectAbility = SelectAbilityEnum.Ability_1});
                isOneAbility = true;
            }
            else
            {
                entity.AddComponent(new SelectPlayerAbilityComponent());
                OpenUISelectAbilityCard(cardComponent);
                return isOneAbility;
            }

            _dataWorld.RiseEvent(new EventUpdateBoardCard());
            return isOneAbility;
        }

        private void OpenUISelectAbilityCard(CardComponent cardComponent)
        {
            var uiSelectAbility = _dataWorld.OneData<UIData>().UIMono.SelectAbilityUIMono;
            var boardGameConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig;
            var cardsImage = _dataWorld.OneData<BoardGameData>().CardsImage;
            var cardsConfig = _dataWorld.OneData<CardsConfig>();
            cardsConfig.Cards.TryGetValue(cardComponent.Key, out var cardConfig);
            
            cardsImage.TryGetValue(cardConfig.ImageKey, out var cardImage);
            
            boardGameConfig.NationsImage.TryGetValue(cardConfig.Nations, out var nationsImage);
            uiSelectAbility.LeftCard.SetViewCard(cardImage, cardConfig.Header, cardConfig.Price, cardConfig.CyberpsychosisCount, nationsImage);
            uiSelectAbility.RightCard.SetViewCard(cardImage, cardConfig.Header, cardConfig.Price, cardConfig.CyberpsychosisCount, nationsImage);
            
            SetViewAbilityCard.SetView(uiSelectAbility.LeftCard.AbilityContainer, cardConfig.Ability_0, boardGameConfig, false, true);
            SetViewAbilityCard.SetView(uiSelectAbility.RightCard.AbilityContainer, cardConfig.Ability_1, boardGameConfig, false, true);
            uiSelectAbility.OpenFrame();
        }
        
        private void SelectFirstAbility()
        {
            SelectConfimAbility(SelectAbilityEnum.Ability_1);
        }

        private void SelectSecondAbility()
        {
            SelectConfimAbility(SelectAbilityEnum.Ability_2);
        }

        private void SelectConfimAbility(SelectAbilityEnum targetAbility)
        {
            var entity = _dataWorld.Select<CardSelectAbilityComponent>().With<SelectPlayerAbilityComponent>().SelectFirstEntity();
            entity.RemoveComponent<CardSelectAbilityComponent>();
            entity.RemoveComponent<SelectPlayerAbilityComponent>();
            entity.AddComponent(new CardTableComponent { SelectAbility = SelectAbilityEnum.Ability_1});
            var uiSelectAbility = _dataWorld.OneData<UIData>().UIMono.SelectAbilityUIMono;
            uiSelectAbility.CloseFrame();
        }
    }
}