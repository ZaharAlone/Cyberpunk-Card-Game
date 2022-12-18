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
    public class BoardGameSetupSystem : IInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;
        private BoardGameData _boardGameData;

        public void Init()
        {
            SetupBoard();
            SetupCard();
            SortingShopCard();
            SortingPlayerCard();
            SortingEnemyCard();
            SetPositionCard();
        }

        //������� ����
        private void SetupBoard()
        {
            var resource = EcsWorldContainer.World.NewEntity();
            var table = Object.Instantiate(_boardGameData.BoardGameConfig.TablePrefab);
            resource.AddComponent(new BoardGameResourceComponent { Table = table });
        }

        //�������������� ��� �����
        private void SetupCard()
        {
            if (!_dataWorld.Select<BoardGameConfigJsonComponent>().Any())
                return;

            _dataWorld.TrySelectFirst<BoardGameConfigJsonComponent>(out var component);

            var cards = Object.Instantiate(new GameObject());
            cards.name = "Cards";

            foreach (var card in component.CardConfig)
            {
                for (var i = 0; i < card.Count; i++)
                {
                    var cardMono = Object.Instantiate(_boardGameData.BoardGameConfig.CardMono, cards.transform);
                    var cardGO = cardMono.gameObject;
                    SetViewCard(cardMono, card);

                    var entity = EcsWorldContainer.World.NewEntity();
                    var cardComponent = SetCardComponent.Set(cardGO, card);
                    entity.AddComponent(cardComponent);

                    if (card.Nations != "Neutral")
                    {
                        cardGO.SetActive(false);
                        entity.AddComponent(new CardCloseComponent());
                    }
                    else
                    {
                        if (!CheckCardIsPlayer(card.Name))
                            entity.AddComponent(new CardNeutralComponent());
                        else
                            SortingCardPlayer(entity, card.Name);
                    }
                }
            }
        }

        //��������� ����������� �� ����� ������
        private bool CheckCardIsPlayer(string Key)
        {
            var isPlayer = false;

            foreach (var item in _boardGameData.BoardGameRule.BasePoolCard)
                if (item.Key == Key)
                    isPlayer = true;
            return isPlayer;
        }

        //������� ����� �������
        private void SortingCardPlayer(Entity entity, string Key)
        {
            var targetCountCard = 0;

            foreach (var item in _boardGameData.BoardGameRule.BasePoolCard)
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

        //������������ ����� ����
        private void SetViewCard(CardMono card, CardStats stats)
        {

        }

        //����� ���� ������� ��� ������� ��������� ����� �� ���� ���� �������.
        #region
        private void SortingShopCard()
        {
            var entities = _dataWorld.Select<CardComponent>()
                             .With<CardCloseComponent>()
                             .GetEntities();

            var count = _dataWorld.Select<CardComponent>()
                                  .With<CardCloseComponent>()
                                  .Count();

            var sorting = SortingCard.Sorting(count);
            var index = 0;

            foreach (var entity in entities)
                entity.GetComponent<CardCloseComponent>().Index = sorting[index];
        }

        private void SortingPlayerCard()
        {
            var entities = _dataWorld.Select<CardComponent>()
                             .With<CardPlayerComponent>()
                             .GetEntities();

            var count = _dataWorld.Select<CardComponent>()
                                  .With<CardPlayerComponent>()
                                  .Count();

            var sorting = SortingCard.Sorting(count);
            var index = 0;

            foreach (var entity in entities)
                entity.GetComponent<CardPlayerComponent>().Index = sorting[index];
        }

        private void SortingEnemyCard()
        {
            var entities = _dataWorld.Select<CardComponent>()
                             .With<CardEnemyComponent>()
                             .GetEntities();

            var count = _dataWorld.Select<CardComponent>()
                                  .With<CardEnemyComponent>()
                                  .Count();

            var sorting = SortingCard.Sorting(count);
            var index = 0;

            foreach (var entity in entities)
                entity.GetComponent<CardEnemyComponent>().Index = sorting[index];
        }
        #endregion

        //������������ ����� �� ������
        private void SetPositionCard()
        {
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