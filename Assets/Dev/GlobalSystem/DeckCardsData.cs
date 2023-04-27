using System.Collections;
using System.Collections.Generic;
using System;

namespace BoardGame.Core
{
    [Serializable]
    public struct DeckCardsData
    {
        public List<CardData> ShopCards;
        public List<CardData> NeutralShopCards;
        public List<CardData> PlayerCards_1;
        public List<CardData> PlayerCards_2;
    }
}