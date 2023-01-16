using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFrameworkUnity;
using System.Collections.Generic;
using UnityEngine;

namespace BoardGame.Core
{
    [EcsSystem(typeof(BoardGameModule))]
    public class BoardGameSetupSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            SetupBoard();
            SetupCard();
            SortingShopCard();
            SortingPlayerCard();
            SortingEnemyCard();
            SetPositionCard();
        }

        //Создаем поле
        private void SetupBoard()
        {
            var resource = EcsWorldContainer.World.NewEntity();
            var boardGameData = _dataWorld.GetOneData<BoardGameData>().GetData();
            var table = Object.Instantiate(boardGameData.BoardGameConfig.TablePrefab);
            resource.AddComponent(new BoardGameResourceComponent { Table = table });
        }

        //Инициализируем все карты
        private void SetupCard()
        {
            var json = _dataWorld.OneData<BoardGameConfigJson>();

            var boardGameData = _dataWorld.GetOneData<BoardGameData>().GetData();
            var cards = new GameObject { name = "Cards" };

            foreach (var card in json.CardConfig)
            {
                for (var i = 0; i < card.Count; i++)
                {
                    var cardMono = Object.Instantiate(boardGameData.BoardGameConfig.CardMono, cards.transform);
                    var cardGO = cardMono.gameObject;
                    SetViewCard(cardMono, card);

                    var entity = EcsWorldContainer.World.NewEntity();
                    var cardComponent = SetCardComponent.Set(cardGO, card, cardMono);
                    entity.AddComponent(cardComponent);

                    if (card.Nations != "Neutral")
                    {
                        entity.AddComponent(new CardInDeckComponent());
                    }
                    else
                    {
                        if (!CheckCardIsPlayer(card.Name))
                            entity.AddComponent(new CardNeutralComponent());
                        else
                            HandingOutCardPlayers(entity, card.Name);
                    }
                }
            }
        }

        //Проверяем принадлежит ли карта игроку
        private bool CheckCardIsPlayer(string Key)
        {
            var isPlayer = false;
            var boardGameData = _dataWorld.GetOneData<BoardGameData>().GetData();

            foreach (var item in boardGameData.BoardGameRule.BasePoolCard)
                if (item.Key == Key)
                    isPlayer = true;
            return isPlayer;
        }

        //Раздаем карты игрокам
        private void HandingOutCardPlayers(Entity entity, string Key)
        {
            var targetCountCard = 0;
            var boardGameData = _dataWorld.GetOneData<BoardGameData>().GetData();

            foreach (var item in boardGameData.BoardGameRule.BasePoolCard)
                if (item.Key == Key)
                    targetCountCard = item.Value;

            var countCardPlayerOne = 0;

            var entities = _dataWorld.Select<CardComponent>()
                                         .With<CardPlayerComponent>()
                                         .GetEntities();

            foreach (var currEntity in entities)
            {
                ref var keyCardPlayer = ref currEntity.GetComponent<CardComponent>().Stats.Name;
                if (keyCardPlayer == Key)
                    countCardPlayerOne++;
            }

            if (countCardPlayerOne < targetCountCard)
                entity.AddComponent(new CardPlayerComponent());
            else
                entity.AddComponent(new CardEnemyComponent());
        }

        //Отрисовываем вьюху кард
        private void SetViewCard(CardMono card, CardStats stats)
        {
            var boardGameData = _dataWorld.GetOneData<BoardGameData>().GetData();
            boardGameData.BoardGameConfig.CardImage.TryGetValue(stats.ImageKey, out var cardImage);

            if (cardImage == null)
                Debug.Log($"Card is name: {stats.ImageKey}");

            if (stats.Nations != "Neutral")
            {
                boardGameData.BoardGameConfig.NationsImage.TryGetValue(stats.Nations, out var nationsImage);
                card.SetViewCard(cardImage, stats.Header, stats.Price, nationsImage);
            }
            else
                card.SetViewCard(cardImage, stats.Header, stats.Price);

            if (stats.Ability.Type != null)
            {
                boardGameData.BoardGameConfig.CurrencyImage.TryGetValue(stats.Ability.Type, out var currencyImage);
                card.SetAbility(currencyImage, stats.Ability.Value);
            }

            if (stats.FractionsAbility.Type != null)
            {
                boardGameData.BoardGameConfig.CurrencyImage.TryGetValue(stats.FractionsAbility.Type, out var currencyImage);
                boardGameData.BoardGameConfig.NationsImage.TryGetValue(stats.Nations, out var nationsImage);
                card.SetFractionAbiltity(nationsImage, currencyImage, stats.FractionsAbility.Value);
            }

            if (stats.DropAbility.Type != null)
            {
                boardGameData.BoardGameConfig.CurrencyImage.TryGetValue(stats.DropAbility.Type, out var currencyImage);
                card.SetAbility(currencyImage, stats.DropAbility.Value);
            }

            card.CardOnBack();
        }

        //Тупой копи пастный код который сортирует карты во всех трех колодах.
        #region
        private void SortingShopCard()
        {
            var entities = _dataWorld.Select<CardComponent>().With<CardInDeckComponent>().GetEntities();
            var count = _dataWorld.Select<CardComponent>().With<CardInDeckComponent>().Count();
            SortingCard.FirstSorting(count, entities);
        }

        private void SortingPlayerCard()
        {
            var entities = _dataWorld.Select<CardComponent>().With<CardPlayerComponent>().GetEntities();
            var count = _dataWorld.Select<CardComponent>().With<CardPlayerComponent>().Count();

            SortingCard.FirstSorting(count, entities);
        }

        private void SortingEnemyCard()
        {
            var entities = _dataWorld.Select<CardComponent>().With<CardEnemyComponent>().GetEntities();
            var count = _dataWorld.Select<CardComponent>().With<CardEnemyComponent>().Count();

            SortingCard.FirstSorting(count, entities);
        }
        #endregion

        //Раскладываем карты по местам
        private void SetPositionCard()
        {
            var boardGameData = _dataWorld.GetOneData<BoardGameData>().GetData();
            var entitiesPlayer = _dataWorld.Select<CardComponent>().With<CardPlayerComponent>().GetEntities();
            foreach (var entity in entitiesPlayer)
                entity.GetComponent<CardComponent>().Transform.position = boardGameData.BoardGameConfig.PositionsCardDeskPlayerOne;

            var entitiesEnemy = _dataWorld.Select<CardComponent>().With<CardEnemyComponent>().GetEntities();
            foreach (var entity in entitiesEnemy)
                entity.GetComponent<CardComponent>().Transform.position = boardGameData.BoardGameConfig.PositionsCardDeskPlayerTwo;

            var entitiesDeck = _dataWorld.Select<CardComponent>().With<CardInDeckComponent>().GetEntities();
            foreach (var entity in entitiesDeck)
                entity.GetComponent<CardComponent>().Transform.position = boardGameData.BoardGameConfig.PositionsShopDeckCard;

            var entitiesNeytral = _dataWorld.Select<CardComponent>().With<CardNeutralComponent>().GetEntities();
            foreach (var entity in entitiesNeytral)
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