using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFrameworkUnity;
using System.Collections.Generic;

namespace BoardGame.Core
{
    [EcsSystem(typeof(LocalGameModule))]
    public class SetupLocalGameSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            SetupCard();
            ModulesUnityAdapter.world.InitModule<BoardGameModule>(true);
        }

        //Инициализируем все карты
        private void SetupCard()
        {
            var json = _dataWorld.OneData<BoardGameConfigJson>();

            var shopCard = new List<PlaceCard>();
            var neutralShopCard = new List<PlaceCard>();
            var player_1 = new List<PlaceCard>();
            var player_2 = new List<PlaceCard>();

            var counterShopCard = 0;
            var counterNeutralCard = 0;
            var counterPlayer_1_Card = 0;
            var counterPlayer_2_Card = 0;

            foreach (var card in json.CardConfig)
            {
                for (var i = 0; i < card.Count; i++)
                {
                    if (card.Nations != "Neutral")
                    {
                        shopCard.Add(new PlaceCard { IDPositions = counterShopCard, CardName = card.Name });
                        counterShopCard++;
                    }
                    else
                    {
                        if (!CheckCardIsPlayer(card.Name))
                        {
                            neutralShopCard.Add(new PlaceCard { IDPositions = counterNeutralCard, CardName = card.Name });
                            counterNeutralCard++;
                        }
                        else
                        {
                            if (CardIsFirstPlayer(player_1, card.Name))
                            {
                                player_1.Add(new PlaceCard { IDPositions = counterPlayer_1_Card, CardName = card.Name });
                                counterPlayer_1_Card++;
                            }
                            else
                            {
                                player_2.Add(new PlaceCard { IDPositions = counterPlayer_2_Card, CardName = card.Name });
                                counterPlayer_2_Card++;
                            }
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
            var boardGameData = _dataWorld.GetOneData<BoardGameData>().GetData();

            foreach (var item in boardGameData.BoardGameRule.BasePoolCard)
                if (item.Key == Key)
                    isPlayer = true;
            return isPlayer;
        }

        private bool CardIsFirstPlayer(List<PlaceCard> playerCard, string Key)
        {
            var targetCountCard = 0;
            var boardGameData = _dataWorld.GetOneData<BoardGameData>().GetData();

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