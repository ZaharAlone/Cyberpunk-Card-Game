using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFrameworkUnity;
using System.Collections.Generic;
using UnityEngine;
using CyberNet.Core.UI;
using Unity.VisualScripting;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class BoardGameSetupSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            SetupBoard();
            SetupCard();
            SetPositionCard();
        }

        //Создаем поле
        private void SetupBoard()
        {
            var resource = ModulesUnityAdapter.world.NewEntity();
            var boardGameData = _dataWorld.OneData<BoardGameData>();
            var table = Object.Instantiate(boardGameData.BoardGameConfig.TablePrefab);
            resource.AddComponent(new BoardGameResourceComponent { Table = table });
        }

        //Инициализируем все карты
        private void SetupCard()
        {
            var deckCardsData = _dataWorld.OneData<DeckCardsData>();
            var cardsParent = new GameObject { name = "Cards" }.transform;

            foreach (var item in deckCardsData.ShopCards)
            {
                var entity = InitCard(item, cardsParent, false);
                entity.AddComponent(new CardTradeDeckComponent());
            }

            foreach (var item in deckCardsData.NeutralShopCards)
            {
                var entity = InitCard(item, cardsParent, false);
                entity.AddComponent(new CardNeutralComponent());
                entity.AddComponent(new CardTradeRowComponent());
            }

            foreach (var item in deckCardsData.PlayerCards_1)
            {
                var entity = InitCard(item, cardsParent, true);
                ref var cardComponent = ref entity.GetComponent<CardComponent>();
                cardComponent.Player = PlayerEnum.Player1;
                entity.AddComponent(new CardDrawComponent());
            }

            foreach (var item in deckCardsData.PlayerCards_2)
            {
                var entity = InitCard(item, cardsParent, true);
                ref var cardComponent = ref entity.GetComponent<CardComponent>();
                cardComponent.Player = PlayerEnum.Player2;
                entity.AddComponent(new CardDrawComponent());
            }
        }

        private Entity InitCard(CardData placeCard, Transform parent, bool isPlayerCard)
        {
            var cardsConfig = _dataWorld.OneData<CardsConfig>();
            var boardGameConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig;
            var cardMono = Object.Instantiate(boardGameConfig.CardUnit, parent);
            var cardGO = cardMono.gameObject;
            cardsConfig.Cards.TryGetValue(placeCard.CardName, out var card);

            if (isPlayerCard)
                cardGO.transform.localScale = boardGameConfig.SizeCardInDeck;

            SetViewCard(cardMono, card);

            var entity = ModulesUnityAdapter.world.NewEntity();
            var cardComponent = SetCardComponent.Set(cardGO, card, cardMono);
            entity.AddComponent(cardComponent);
            entity.AddComponent(new CardSortingIndexComponent { Index = placeCard.IDPositions });
            return entity;
        }

        //Отрисовываем вьюху карт
        private void SetViewCard(CardMono card, CardConfig cardConfig)
        {
            var boardGameConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig;
            var cardsImage = _dataWorld.OneData<BoardGameData>().CardsImage;
            cardsImage.TryGetValue(cardConfig.ImageKey, out var cardImage);

            if (cardImage == null)
            {
                Debug.LogError($"Not find card image is name: {cardConfig.ImageKey}");
                return;
            }

            if (cardConfig.Nations != "Neutral")
            {
                boardGameConfig.NationsImage.TryGetValue(cardConfig.Nations, out var nationsImage);
                card.SetViewCard(cardImage, cardConfig.Header, cardConfig.CyberpsychosisCount, cardConfig.Price, nationsImage);

                for (int i = 0; i < cardConfig.Count; i++)
                    Object.Instantiate(boardGameConfig.ItemIconsCounterCard, card.CountCardBlock);
            }
            else
                card.SetViewCard(cardImage, cardConfig.Header, cardConfig.CyberpsychosisCount, cardConfig.Price);

            var isAbility_0 = cardConfig.Ability_0.AbilityType != AbilityType.None;
            var isAbility_1 = cardConfig.Ability_1.AbilityType != AbilityType.None;
            var isAbility_2 = cardConfig.Ability_2.AbilityType != AbilityType.None;
            var onlyOneAbility = isAbility_0 && !isAbility_1 && !isAbility_2;
            var chooseAbility = isAbility_0 && isAbility_1;

            card.SetChooseAbility(chooseAbility);
            card.IsConditionAbility(isAbility_2);
            
            if (!chooseAbility)
                card.AbilityBlock_2_Container.GameObject().SetActive(false);
            
            if (chooseAbility && isAbility_2)
                card.SetBigDownBlock();
            
            if (cardConfig.Ability_0.AbilityType != AbilityType.None)
            {
                if (onlyOneAbility)
                    SetViewAbilityCard.SetView(card.AbilityBlock_OneShot_Container, cardConfig.Ability_0, boardGameConfig, chooseAbility, onlyOneAbility);
                else
                    SetViewAbilityCard.SetView(card.AbilityBlock_1_Container, cardConfig.Ability_0, boardGameConfig, chooseAbility);
            }
            if (cardConfig.Ability_1.AbilityType != AbilityType.None)
                SetViewAbilityCard.SetView(card.AbilityBlock_2_Container, cardConfig.Ability_1, boardGameConfig, chooseAbility);
            if (cardConfig.Ability_2.AbilityType != AbilityType.None)
                SetViewAbilityCard.SetView(card.AbilityBlock_3_Container, cardConfig.Ability_2, boardGameConfig);

            card.CardOnBack();
        }

        //Раскладываем карты по местам
        private void SetPositionCard()
        {
            var gameUI = _dataWorld.OneData<UIData>();
            var entitiesPlayer1 = _dataWorld.Select<CardComponent>()
                                            .Where<CardComponent>(card => card.Player == PlayerEnum.Player1)
                                            .GetEntities();


            foreach (var entity in entitiesPlayer1)
            {
                ref var component = ref entity.GetComponent<CardComponent>();
                component.Transform.position = gameUI.UIMono.DownDeck.localPosition;
                component.CardMono.HideCard();
                component.CardMono.HideBackCardColor();
            }

            var entitiesPlayer2 = _dataWorld.Select<CardComponent>()
                                            .Where<CardComponent>(card => card.Player == PlayerEnum.Player2)
                                            .GetEntities();
            foreach (var entity in entitiesPlayer2)
            {
                ref var component = ref entity.GetComponent<CardComponent>();
                component.Transform.position = gameUI.UIMono.UpDeck.localPosition;
                component.CardMono.HideCard();
                component.CardMono.HideBackCardColor();
            }

            var entitiesDeck = _dataWorld.Select<CardComponent>().With<CardTradeDeckComponent>().GetEntities();
            foreach (var entity in entitiesDeck)
                entity.GetComponent<CardComponent>().CardMono.HideCard();

            var positionsNeutralCard = new Vector2(Screen.resolutions.Length / 2 - 560, 200);
            var entitiesNeutral = _dataWorld.Select<CardComponent>().With<CardNeutralComponent>().GetEntities();
            foreach (var entity in entitiesNeutral)
            {
                ref var component = ref entity.GetComponent<CardComponent>();
                component.Transform.position = positionsNeutralCard;
                component.CardMono.CardOnFace();
            }
        }

        public void Destroy()
        {
            var entities = _dataWorld.Select<BoardGameResourceComponent>()
                                     .GetEntities();

            foreach (var entity in entities)
            {
                var resource = entity.GetComponent<BoardGameResourceComponent>();
                Object.Destroy(resource.Table);
            }
        }
    }
}