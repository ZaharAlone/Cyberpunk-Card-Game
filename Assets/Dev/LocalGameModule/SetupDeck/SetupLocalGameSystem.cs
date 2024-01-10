using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFrameworkUnity;
using System.Collections.Generic;
using System.Linq;
using CyberNet.Global;
using UnityEngine;

namespace CyberNet.Core
{
    [EcsSystem(typeof(LocalGameModule))]
    public class SetupLocalGameSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            SetupCard();
            ModulesUnityAdapter.world.InitModule<CoreModule>(true);
        }

        //Инициализируем все карты
        private void SetupCard()
        {
            var cardsConfig = _dataWorld.OneData<CardsConfig>();
            ref var playerSelect = ref _dataWorld.OneData<SelectPlayerData>().SelectLeaders;
            var shopCard = new List<CardData>();
            var neutralShopCard = new List<CardData>();
            var playerDeckCard = new List<PlayerCardData>();

            foreach (var player in playerSelect)
            {
                playerDeckCard.Add(new PlayerCardData {
                    IndexPlayer = player.PlayerID,
                    Cards = new()
                });
            }

            foreach (var cardKeyValue in cardsConfig.Cards)
            {
                var card = cardKeyValue.Value;
                for (var i = 0; i < card.Count; i++)
                {
                    if (card.Nations != "Neutral")
                        shopCard.Add(new CardData { IDPositions = shopCard.Count, CardName = card.Name });
                    else
                    {
                        if (!CheckCardIsPlayer(card.Name))
                        {
                            neutralShopCard.Add(new CardData {
                                IDPositions = neutralShopCard.Count,
                                CardName = card.Name
                            });
                        }
                        else
                        {
                            var selectPlayerIndex = WhatPlayerGetCard(playerDeckCard, card.Name);
                            
                            if (selectPlayerIndex == -1)
                                continue;
                            playerDeckCard[selectPlayerIndex].Cards.Add(new CardData 
                            {
                                IDPositions = playerDeckCard[selectPlayerIndex].Cards.Count(),
                                CardName = card.Name
                            });
                        }
                    }
                }
            }

            shopCard = SortingCard.SortingDeckCards(shopCard);
            
            var sortingPlayerDeckCard = new List<PlayerCardData>();
            foreach (var playerDeck in playerDeckCard)
            {
                sortingPlayerDeckCard.Add(new PlayerCardData {
                    Cards = SortingCard.SortingDeckCards(playerDeck.Cards),
                    IndexPlayer = playerDeck.IndexPlayer
                });
            }

            _dataWorld.CreateOneData(new DeckCardsData {
                ShopCards = shopCard,
                NeutralShopCards = neutralShopCard,
                PlayerDeckCard = sortingPlayerDeckCard
            });
        }

        private bool CheckCardIsPlayer(string Key)
        {
            var isPlayer = false;
            var boardGameData = _dataWorld.OneData<BoardGameData>();

            foreach (var item in boardGameData.BoardGameRule.BasePoolCard)
                if (item.Item == Key)
                    isPlayer = true;
            return isPlayer;
        }

        //Какому по индексу игроку подходит данная карта?
        private int WhatPlayerGetCard(List<PlayerCardData> playerDeckCard, string KeyCard)
        {
            var targetCountCard = 0;
            var boardGameData = _dataWorld.OneData<BoardGameData>();

            foreach (var item in boardGameData.BoardGameRule.BasePoolCard)
                if (item.Item == KeyCard)
                    targetCountCard = item.Value;
            
            var selectPlayerIndex = 0;
            foreach (var playerDeck in playerDeckCard)
            {
                var countCardSelectPlayer = 0;
                foreach (var playerCard in playerDeck.Cards)
                {
                    if (playerCard.CardName == KeyCard)
                        countCardSelectPlayer++;
                }

                if (countCardSelectPlayer < targetCountCard)
                    return selectPlayerIndex;
                else
                    selectPlayerIndex++;
            }
            return -1;
        }
    }
}