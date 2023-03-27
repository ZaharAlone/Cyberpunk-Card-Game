using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFrameworkUnity;
using System.Collections.Generic;
using UnityEngine;

namespace BoardGame.Core
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
            var boardGameData = _dataWorld.GetOneData<BoardGameData>().GetData();
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
                var entity = InitCard(item, cardsParent);
                entity.AddComponent(new CardTradeDeckComponent());
            }

            foreach (var item in deckCardsData.NeutralShopCards)
            {
                var entity = InitCard(item, cardsParent);
                entity.AddComponent(new CardNeutralComponent());
            }

            foreach (var item in deckCardsData.PlayerCards_1)
            {
                var entity = InitCard(item, cardsParent);
                entity.AddComponent(new CardPlayer1Component());
            }

            foreach (var item in deckCardsData.PlayerCards_2)
            {
                var entity = InitCard(item, cardsParent);
                entity.AddComponent(new CardPlayer2Component());
            }
        }

        private Entity InitCard(CardData placeCard, Transform parent)
        {
            var cardsConfig = _dataWorld.OneData<CardsConfig>();
            var boardGameConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig;
            var cardMono = Object.Instantiate(boardGameConfig.CardMono, parent);
            var cardGO = cardMono.gameObject;
            cardsConfig.Cards.TryGetValue(placeCard.CardName, out var card);

            SetViewCard(cardMono, card);

            var entity = ModulesUnityAdapter.world.NewEntity();
            var cardComponent = SetCardComponent.Set(cardGO, card, cardMono);
            entity.AddComponent(cardComponent);
            entity.AddComponent(new CardSortingIndexComponent { Index = placeCard.IDPositions });
            return entity;
        }

        //Отрисовываем вьюху кард
        private void SetViewCard(CardMono card, CardConfig cardConfig)
        {
            var boardGameData = _dataWorld.GetOneData<BoardGameData>().GetData();
            boardGameData.BoardGameConfig.CardImage.TryGetValue(cardConfig.ImageKey, out var cardImage);

            if (cardImage == null)
                Debug.LogError($"Not find card image is name: {cardConfig.ImageKey}");

            if (cardConfig.Nations != "Neutral")
            {
                boardGameData.BoardGameConfig.NationsImage.TryGetValue(cardConfig.Nations, out var nationsImage);
                card.SetViewCard(cardImage, cardConfig.Header, cardConfig.CyberpsychosisCount, cardConfig.Price, nationsImage);
            }
            else
                card.SetViewCard(cardImage, cardConfig.Header, cardConfig.CyberpsychosisCount, cardConfig.Price);

            if (cardConfig.Ability_0.Action != AbilityAction.None)
                SetViewAbilityCard.SetView(card.Ability_0_Container, cardConfig.Ability_0, boardGameData.BoardGameConfig);
            if (cardConfig.Ability_1.Action != AbilityAction.None)
                SetViewAbilityCard.SetView(card.Ability_1_Container, cardConfig.Ability_1, boardGameData.BoardGameConfig);

            card.CardOnBack();
        }

        //Раскладываем карты по местам
        private void SetPositionCard()
        {
            var boardGameData = _dataWorld.OneData<BoardGameData>();
            var targetSizeDeckCard = boardGameData.BoardGameConfig.SizeCardInDeck;

            var entitiesPlayer = _dataWorld.Select<CardComponent>().With<CardPlayer1Component>().GetEntities();
            foreach (var entity in entitiesPlayer)
            {
                ref var component = ref entity.GetComponent<CardComponent>();
                component.Transform.position = boardGameData.BoardGameConfig.PositionsCardDeckPlayer;
                component.Transform.localScale = targetSizeDeckCard;
                component.GO.SetActive(false);
            }

            var entitiesEnemy = _dataWorld.Select<CardComponent>().With<CardPlayer2Component>().GetEntities();
            foreach (var entity in entitiesEnemy)
            {
                ref var component = ref entity.GetComponent<CardComponent>();
                component.Transform.position = boardGameData.BoardGameConfig.PositionsCardDeckEnemy;
                component.Transform.localScale = targetSizeDeckCard;
                component.GO.SetActive(false);
            }

            var entitiesDeck = _dataWorld.Select<CardComponent>().With<CardTradeDeckComponent>().GetEntities();
            foreach (var entity in entitiesDeck)
                entity.GetComponent<CardComponent>().CardMono.HideCard();

            var entitiesNeutral = _dataWorld.Select<CardComponent>().With<CardNeutralComponent>().GetEntities();
            foreach (var entity in entitiesNeutral)
            {
                ref var component = ref entity.GetComponent<CardComponent>();
                component.Transform.position = boardGameData.BoardGameConfig.PositionsShopNeutralCard;
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