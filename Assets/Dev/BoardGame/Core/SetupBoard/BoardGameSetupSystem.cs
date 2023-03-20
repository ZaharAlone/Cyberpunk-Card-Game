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
            SetPositionCard();
        }

        //������� ����
        private void SetupBoard()
        {
            var resource = ModulesUnityAdapter.world.NewEntity();
            var boardGameData = _dataWorld.GetOneData<BoardGameData>().GetData();
            var table = Object.Instantiate(boardGameData.BoardGameConfig.TablePrefab);
            resource.AddComponent(new BoardGameResourceComponent { Table = table });
        }

        //�������������� ��� �����
        private void SetupCard()
        {
            var json = _dataWorld.OneData<BoardGameConfigJson>();
            var boardGameData = _dataWorld.OneData<BoardGameData>();
            var deckCardsData = _dataWorld.OneData<DeckCardsData>();

            var cards = new GameObject { name = "Cards" };
            /*
            foreach (var card in deckCardsData.NeutralShopCards)
            {
                var cardMono = Object.Instantiate(boardGameData.BoardGameConfig.CardMono, cards.transform);
                var cardGO = cardMono.gameObject;
                SetViewCard(cardMono, card);

                var entity = ModulesUnityAdapter.world.NewEntity();
                var cardComponent = SetCardComponent.Set(cardGO, card, cardMono);
                entity.AddComponent(cardComponent);

                if (card.Nations != "Neutral")
                {
                    entity.AddComponent(new CardTradeDeckComponent());
                }
                else
                {
                    if (!CheckCardIsPlayer(card.Name))
                        entity.AddComponent(new CardNeutralComponent());
                    else
                        HandingOutCardPlayers(entity, card.Name);
                }
            }*/
        }

        //��������� ����������� �� ����� ������
        private bool CheckCardIsPlayer(string Key)
        {
            var isPlayer = false;
            var boardGameData = _dataWorld.GetOneData<BoardGameData>().GetData();

            foreach (var item in boardGameData.BoardGameRule.BasePoolCard)
                if (item.Key == Key)
                    isPlayer = true;
            return isPlayer;
        }

        //������� ����� �������
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

        //������������ ����� ����
        private void SetViewCard(CardMono card, CardStats stats)
        {
            var boardGameData = _dataWorld.GetOneData<BoardGameData>().GetData();
            boardGameData.BoardGameConfig.CardImage.TryGetValue(stats.ImageKey, out var cardImage);

            if (cardImage == null)
                Debug.LogError($"Not find card image is name: {stats.ImageKey}");

            if (stats.Nations != "Neutral")
            {
                boardGameData.BoardGameConfig.NationsImage.TryGetValue(stats.Nations, out var nationsImage);
                card.SetViewCard(cardImage, stats.Header, stats.CyberpsychosisCount, stats.Price, nationsImage);
            }
            else
                card.SetViewCard(cardImage, stats.Header, stats.CyberpsychosisCount, stats.Price);

            if (stats.Ability_0.Action != AbilityAction.None)
                SetViewAbilityCard.SetView(card.Ability_0_Container, stats.Ability_0, boardGameData.BoardGameConfig);
            if (stats.Ability_1.Action != AbilityAction.None)
                SetViewAbilityCard.SetView(card.Ability_1_Container, stats.Ability_1, boardGameData.BoardGameConfig);

            card.CardOnBack();
        }

        //������������ ����� �� ������
        private void SetPositionCard()
        {
            var boardGameData = _dataWorld.OneData<BoardGameData>();
            var targetSizeDeckCard = boardGameData.BoardGameConfig.SizeCardInDeck;

            var entitiesPlayer = _dataWorld.Select<CardComponent>().With<CardPlayerComponent>().GetEntities();
            foreach (var entity in entitiesPlayer)
            {
                ref var component = ref entity.GetComponent<CardComponent>();
                component.Transform.position = boardGameData.BoardGameConfig.PositionsCardDeckPlayer;
                component.Transform.localScale = targetSizeDeckCard;
                component.GO.SetActive(false);
            }

            var entitiesEnemy = _dataWorld.Select<CardComponent>().With<CardEnemyComponent>().GetEntities();
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