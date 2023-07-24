using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFrameworkUnity;
using System.Collections.Generic;
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

            var shopCard = new List<CardData>();
            var neutralShopCard = new List<CardData>();
            var player_1 = new List<CardData>();
            var player_2 = new List<CardData>();

            foreach (var cardKV in cardsConfig.Cards)
            {
                var card = cardKV.Value;
                for (var i = 0; i < card.Count; i++)
                {
                    if (card.Nations != "Neutral")
                        shopCard.Add(new CardData { IDPositions = shopCard.Count, CardName = card.Name });
                    else
                    {
                        if (!CheckCardIsPlayer(card.Name))
                            neutralShopCard.Add(new CardData { IDPositions = neutralShopCard.Count, CardName = card.Name });
                        else
                        {
                            if (CardIsFirstPlayer(player_1, card.Name))
                                player_1.Add(new CardData { IDPositions = player_1.Count, CardName = card.Name });
                            else
                                player_2.Add(new CardData { IDPositions = player_2.Count, CardName = card.Name });
                        }
                    }
                }
            }

            SortingCard.SortingDeckCards(shopCard);
            SortingCard.SortingDeckCards(player_1);
            SortingCard.SortingDeckCards(player_2);

            _dataWorld.CreateOneData(new DeckCardsData { ShopCards = shopCard, NeutralShopCards = neutralShopCard, PlayerCards_1 = player_1, PlayerCards_2 = player_2 });
        }

        private bool CheckCardIsPlayer(string Key)
        {
            var isPlayer = false;
            var boardGameData = _dataWorld.OneData<BoardGameData>();

            foreach (var item in boardGameData.BoardGameRule.BasePoolCard)
                if (item.Key == Key)
                    isPlayer = true;
            return isPlayer;
        }

        private bool CardIsFirstPlayer(List<CardData> playerCard, string Key)
        {
            var targetCountCard = 0;
            var boardGameData = _dataWorld.OneData<BoardGameData>();

            foreach (var item in boardGameData.BoardGameRule.BasePoolCard)
                if (item.Key == Key)
                    targetCountCard = item.Value;

            var countCardPlayerOne = 0;

            foreach (var card in playerCard)
                if (card.CardName == Key)
                    countCardPlayerOne++;

            return countCardPlayerOne < targetCountCard;
        }
    }
}