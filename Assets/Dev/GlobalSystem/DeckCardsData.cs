using System.Collections;
using System.Collections.Generic;
using System;

namespace BoardGame.Core
{
    [Serializable]
    public struct DeckCardsData
    {
        public List<PlaceCard> ShopCards;
        public List<PlaceCard> NeutralShopCards;
        public List<PlaceCard> PlayerCards_1;
        public List<PlaceCard> PlayerCards_2;
    }
}